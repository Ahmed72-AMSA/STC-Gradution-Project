import React, { useState, useEffect, useCallback, useRef } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Button, Spinner, Alert, Badge, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { FiSend, FiUser, FiX, FiMessageSquare, FiCheck, FiCheckCircle } from 'react-icons/fi';
import './Chat.css';

const Chat = () => {
  const [conn, setConnection] = useState(null);
  const [messages, setMessages] = useState({});
  const [message, setMessage] = useState('');
  const [isConnected, setIsConnected] = useState(false);
  const [onlineUsers, setOnlineUsers] = useState([]);
  const [username, setUsername] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeChat, setActiveChat] = useState(null);
  const messagesEndRef = useRef(null);
  const apiBaseUrl = 'https://localhost:7152';

  // Fetch user info on component mount
  useEffect(() => {
    const fetchUserInfo = async () => {
      try {
        const storedId = localStorage.getItem('userId');
        if (!storedId) {
          throw new Error('Please login to access chat');
        }

        const response = await fetch(`${apiBaseUrl}/api/Users/BasicInfo/${storedId}`);
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        if (!data.username) {
          throw new Error('Username not found in response');
        }
        
        setUsername(data.username);
      } catch (err) {
        console.error('Error fetching user info:', err);
        setError(err.message || 'Failed to load user information');
      } finally {
        setLoading(false);
      }
    };

    fetchUserInfo();
  }, [apiBaseUrl]);

  // Scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages, activeChat]);

  // Handle new messages
  const handleNewMessage = useCallback((sender, message) => {
    setMessages(prev => {
      const chatKey = sender === username ? activeChat : sender;
      const existingMessages = prev[chatKey] || [];
      
      return {
        ...prev,
        [chatKey]: [
          ...existingMessages,
          { 
            sender, 
            message, 
            timestamp: new Date(),
            isCurrentUser: sender === username,
            isRead: sender === username // Messages you send are automatically "read"
          }
        ]
      };
    });
  }, [username, activeChat]);

  // Handle message history
  const handleMessageHistory = useCallback((history) => {
    const formattedMessages = history.map(msg => ({
      sender: msg.senderUsername,
      message: msg.content,
      timestamp: new Date(msg.sentAt),
      isCurrentUser: msg.senderUsername === username,
      isRead: msg.isRead
    }));
    
    setMessages(prev => ({
      ...prev,
      [activeChat]: formattedMessages
    }));
  }, [username, activeChat]);

  // Setup SignalR connection
  useEffect(() => {
    if (!username) return;

    const connection = new HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/chat`)
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          return 5000; // Retry every 5 seconds
        }
      })
      .build();

    // Event handlers
    connection.on("ReceivePrivateMessage", handleNewMessage);
    connection.on("UpdateUserList", users => setOnlineUsers(users));
    connection.on("LoadMessageHistory", handleMessageHistory);
    connection.on("UserNotFound", targetUser => {
      setError(`${targetUser} is not available`);
    });

    const startConnection = async () => {
      try {
        await connection.start();
        await connection.invoke("ConnectUser", username);
        setConnection(connection);
        setIsConnected(true);
        setError(null);
      } catch (err) {
        console.error('Connection error:', err);
        setError('Connection failed. Trying to reconnect...');
      }
    };

    startConnection();

    return () => {
      if (connection) {
        connection.off("ReceivePrivateMessage", handleNewMessage);
        connection.off("LoadMessageHistory", handleMessageHistory);
        connection.stop().catch(err => console.error('Error stopping connection:', err));
      }
    };
  }, [username, handleNewMessage, handleMessageHistory, apiBaseUrl]);

  const loadMessageHistory = useCallback(async (targetUser) => {
    if (conn && isConnected) {
      try {
        await conn.invoke("GetMessageHistory", username, targetUser);
      } catch (err) {
        console.error('Error loading message history:', err);
        setError('Failed to load message history');
      }
    }
  }, [conn, isConnected, username]);

  const sendMessage = useCallback(async () => {
    if (!message.trim() || !isConnected || !activeChat) return;

    try {
      await conn.invoke("SendPrivateMessage", activeChat, username, message);
      setMessage('');
    } catch (error) {
      console.error('Error sending message:', error);
      setError('Message send failed');
    }
  }, [message, isConnected, activeChat, conn, username]);

  const openChat = useCallback((user) => {
    setActiveChat(user);
    if (!messages[user]) {
      loadMessageHistory(user);
    }
  }, [loadMessageHistory, messages]);

  const handleKeyDown = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  };

  const formatTime = (date) => {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  if (loading) {
    return (
      <div className="chat-loader">
        <Spinner animation="border" variant="primary" />
        <p className="mt-3">Loading chat...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="chat-error">
        <Alert variant="danger" className="text-center">
          <Alert.Heading>Error</Alert.Heading>
          <p>{error}</p>
          <Button 
            variant="primary" 
            onClick={() => window.location.reload()}
            className="mt-2"
          >
            Reconnect
          </Button>
        </Alert>
      </div>
    );
  }

  return (
    <div className="chat-app">
      <div className="chat-sidebar">
        <div className="user-header">
          <div className="user-avatar">
            <FiUser size={24} />
          </div>
          <div className="user-info">
            <h5>{username}</h5>
            <Badge bg="primary" pill>
              User
            </Badge>
          </div>
        </div>

        <div className="online-users">
          <h6>Online Users</h6>
          <ul>
            {onlineUsers.filter(user => user !== username).map((user) => (
              <li 
                key={user}
                className={activeChat === user ? 'active' : ''}
                onClick={() => openChat(user)}
              >
                <span className="user-avatar-sm">
                  <FiUser size={16} />
                </span>
                <span className="user-name">
                  {user}
                </span>
                <span className="user-status online"></span>
              </li>
            ))}
          </ul>
        </div>
      </div>

      <div className="chat-main">
        {activeChat ? (
          <div className="chat-window">
            <div className="chat-header">
              <div className="chat-partner">
                <span className="user-avatar-sm">
                  <FiUser size={18} />
                </span>
                <h5>{activeChat}</h5>
              </div>
              <Button 
                variant="link" 
                onClick={() => setActiveChat(null)}
                className="close-chat"
                aria-label="Close chat"
              >
                <FiX size={20} />
              </Button>
            </div>

            <div className="messages-container">
              {messages[activeChat]?.map((msg, idx) => (
                <div 
                  key={`${msg.sender}-${idx}`}
                  className={`message ${msg.isCurrentUser ? 'sent' : 'received'}`}
                >
                  <div className="message-content">
                    {!msg.isCurrentUser && (
                      <div className="message-sender">{msg.sender}</div>
                    )}
                    <div className="message-text">{msg.message}</div>
                    <div className="message-time">
                      {formatTime(msg.timestamp)}
                      {msg.isCurrentUser && (
                        <OverlayTrigger
                          placement="top"
                          overlay={
                            <Tooltip>
                              {msg.isRead ? 'Read' : 'Delivered'}
                            </Tooltip>
                          }
                        >
                          <span className="read-receipt">
                            {msg.isRead ? <FiCheckCircle /> : <FiCheck />}
                          </span>
                        </OverlayTrigger>
                      )}
                    </div>
                  </div>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </div>

            <div className="message-input">
              <Form.Control
                as="textarea"
                rows={3}
                placeholder="Type a message..."
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                onKeyDown={handleKeyDown}
              />
              <Button 
                variant="primary" 
                onClick={sendMessage}
                disabled={!message.trim() || !isConnected}
                aria-label="Send message"
              >
                <FiSend size={18} />
              </Button>
            </div>
          </div>
        ) : (
          <div className="chat-welcome">
            <div className="welcome-content">
              <FiMessageSquare size={48} className="mb-3" />
              <h4>Welcome to Chat</h4>
              <p>Select a user to start chatting</p>
              {onlineUsers.length <= 1 && (
                <Alert variant="warning" className="mt-3">
                  No other users are currently online
                </Alert>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Chat;