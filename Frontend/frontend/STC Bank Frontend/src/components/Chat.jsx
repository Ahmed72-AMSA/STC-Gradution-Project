import React, { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Row, Col, Form, Button } from 'react-bootstrap';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import './Chat.css';

const Chat = () => {
  const [conn, setConnection] = useState(null);
  const [messages, setMessages] = useState([]);
  const [username, setUsername] = useState('');
  const [message, setMessage] = useState('');
  const [recipient, setRecipient] = useState('');
  const [isConnected, setIsConnected] = useState(false);
  const [openChats, setOpenChats] = useState([]);

  useEffect(() => () => conn?.stop(), [conn]);

  const joinChatRoom = async () => {
    if (!username) return alert('Please enter a username.');
    try {
      const connection = new HubConnectionBuilder()
        .withUrl('http://localhost:5001/chat')
        .configureLogging(LogLevel.Information)
        .build();

      connection.on('UserConnected', (user) => console.log(`${user} has connected.`));
      connection.on('ReceivePrivateMessage', (sender, msg) => setMessages(prev => [...prev, { sender, msg, recipient: username }]));
      connection.on('ShowMessages', setMessages);

      await connection.start();
      await connection.invoke('ConnectUser', username);
      setConnection(connection);
      setIsConnected(true);
    } catch (error) {
      console.error('Error connecting:', error);
      alert('Failed to join chat room.');
    }
  };

  useEffect(() => {
    const fetchMessages = async () => {
      try {
        if (isConnected && conn) await conn.invoke('GetUserMessages', username);
      } catch (error) {
        console.error('Error fetching messages:', error);
      }
    };

    const interval = setInterval(fetchMessages, 2000);
    return () => clearInterval(interval);
  }, [isConnected, conn, username]);

  const sendMessage = async () => {
    if (!message.trim()) return alert('Please enter a message.');
    if (!isConnected) return alert('Not connected to chat room.');
    try {
      await conn.invoke('SendPrivateMessage', recipient, username, message);
      setMessages(prev => [...prev, { sender: username, msg: message, recipient }]);
      setMessage('');
    } catch (error) {
      console.error('Error sending:', error);
      alert('Failed to send message.');
    }
  };

  const openChat = (recipient) => {
    setRecipient(recipient);
    setMessages([]);
    setOpenChats(prevChats => [...prevChats, recipient]);
  };

  const closeChat = (recipient) => {
    setOpenChats(prevChats => prevChats.filter(chat => chat !== recipient));
  };

  const createNewChat = () => {
    if (!recipient) return alert('Please enter a recipient username.');
    if (openChats.includes(recipient)) return alert('You are already chatting with this user.');
    openChat(recipient);
  };

  return (
    <diV className="chat-app">
    <Container className="chat-container mt-5">
      {!isConnected ? (
        <Row className="my-4">
          <Col sm="12" md="4" className="mx-auto">
            <Form>
              <Form.Group controlId="formUsername">
                <Form.Label>Username</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter your username"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  className="form-input"
                />
              </Form.Group>
              <Button variant="primary" onClick={joinChatRoom} className="w-100 send-btn">
                Join Chat
              </Button>
            </Form>
          </Col>
        </Row>
      ) : (
        <Row className="my-4">
          <Col sm="12" md="12">
            {openChats.length === 0 ? (
              <div className="users-list">
                {messages
                  .filter((msg, index, self) => self.findIndex(m => m.sender === msg.sender) === index)
                  .map((msg, msgIndex) => (
                    msg.recipient === username && !openChats.includes(msg.sender) ? (
                      <Button
                        key={msgIndex}
                        variant="outline-primary"
                        onClick={() => openChat(msg.sender)}
                        className="w-100 open-chat-btn mb-5"
                      >
                        Chat with {msg.sender}
                      </Button>
                    ) : null
                  ))}
                <div className="new-chat-container my-5">
                  <Form.Group controlId="formRecipient">
                    <Form.Label className='f'>Enter Username to Start a New Chat</Form.Label>
                    <Form.Control
                      type="text"
                      placeholder="Enter recipient's username"
                      value={recipient}
                      onChange={(e) => setRecipient(e.target.value)}
                      className="form-input"
                    />
                  </Form.Group>
                  <Button variant="outline-info" onClick={createNewChat} className="w-100 new-chat-btn mt-5">
                    Start New Chat
                  </Button>
                </div>
              </div>
            ) : (
              openChats.map((chat, index) => (
                <div key={index} className="chat-box">
                  <Row className="chat-header">
                    <Col sm="12">
                      <span className='chat-header-name'>{chat}</span>
                      <Button variant="danger" onClick={() => closeChat(chat)} className="close-chat-btn">X</Button>
                    </Col>
                  </Row>
                  <Row className="chat-messages my-4">
                    <Col sm="12">
                      <div className="messages-list">
                        {messages.filter(msg => msg.sender === chat || msg.recipient === chat).map((msg, msgIndex) => (
                          <div key={msgIndex} className={`message ${msg.sender === username ? 'sent' : 'received'}`}>
                            <div className="message-content">
                              <strong>{msg.sender}</strong>
                              <p>{msg.text}</p>
                            </div>
                          </div>
                        ))}
                      </div>
                    </Col>
                  </Row>

                  <Row className="my-4">
                    <Col sm="12" md="8" className="mx-auto">
                      <Form.Control
                        type="text"
                        placeholder="Type a message"
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                        onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                        className="message-input"
                      />
                      
                    </Col>
                  </Row>
                </div>
              ))
            )}
          </Col>
        </Row>
      )}
    </Container>
    </diV>
  );
};

export default Chat;
