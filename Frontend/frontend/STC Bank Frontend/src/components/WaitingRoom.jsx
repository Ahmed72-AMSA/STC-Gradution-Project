import React, { useState, useEffect } from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Row, Col, Form, Button } from 'react-bootstrap';
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

const Chat = () => {
  const [conn, setConnection] = useState(null);
  const [messages, setMessages] = useState([]); // Store the chat messages
  const [username, setUsername] = useState("");
  const [targetUser, setTargetUser] = useState(""); // Target user for private chat
  const [message, setMessage] = useState("");

  useEffect(() => {
    // Initialize SignalR connection
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5001/chat", { withCredentials: false })
      .configureLogging(LogLevel.Information)
      .build();

    connection.start()
      .then(() => {
        console.log("Connected to SignalR");
      })
      .catch((error) => {
        console.error("Connection failed: ", error);
        alert("Failed to connect to the chat.");
      });

    connection.on("UserConnected", (username) => {
      console.log(`${username} has connected`);
    });

    connection.on("ReceivePrivateMessage", (sender, message) => {
      console.log(`Received message from ${sender}: ${message}`);
      setMessages((prevMessages) => [...prevMessages, { sender, message }]);
    });

    connection.on("Error", (error) => {
      alert(error);
    });

    connection.on("ShowMessages", (messages) => {
      setMessages(messages); // Display all previous messages
    });

    setConnection(connection); // Store connection in state

    return () => {
      // Cleanup on component unmount
      connection.stop();
    };
  }, []);

  const connectUser = async () => {
    if (!username) {
      alert("Please enter your username.");
      return;
    }

    try {
      await conn.invoke("ConnectUser", username); // Connect user to SignalR
    } catch (error) {
      console.error("Error connecting to chat:", error);
      alert("Failed to connect to the chat.");
    }
  };

  const sendMessage = async () => {
    if (!message || !targetUser) {
      alert("Please enter a message and a recipient.");
      return;
    }

    try {
      await conn.invoke("SendPrivateMessage", targetUser, username, message); // Send private message
      setMessage(""); // Clear message input after sending
    } catch (error) {
      console.error("Error sending message:", error);
      alert("Failed to send message.");
    }
  };

  const fetchMessages = async () => {
    if (!username) {
      alert("Please enter your username.");
      return;
    }

    try {
      await conn.invoke("GetUserMessages", username); // Fetch user messages
    } catch (error) {
      console.error("Error fetching messages:", error);
      alert("Failed to fetch messages.");
    }
  };

  return (
    <Container>
      <Row className="px-5 my-5">
        <Col sm="12">
          <h1>Welcome to Private Chat</h1>
        </Col>
      </Row>

      {!conn ? (
        <Row className="my-4">
          <Col sm="12" md="4">
            <Form>
              <Form.Group controlId="formUsername">
                <Form.Label>Username</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter your username"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                />
              </Form.Group>
              <Button variant="primary" onClick={connectUser}>
                Connect
              </Button>
            </Form>
          </Col>
        </Row>
      ) : (
        <>
          <Row className="my-4">
            <Col sm="12">
              <div className="chat-messages" style={{ maxHeight: "300px", overflowY: "auto" }}>
                {messages.map((msg, index) => (
                  <div key={index} className="message">
                    <strong>{msg.sender}:</strong> {msg.Text}
                  </div>
                ))}
              </div>
            </Col>
          </Row>

          <Row className="my-4">
            <Col sm="12" md="6">
              <Form.Control
                type="text"
                placeholder="Target User"
                value={targetUser}
                onChange={(e) => setTargetUser(e.target.value)}
              />
            </Col>
          </Row>

          <Row className="my-4">
            <Col sm="12">
              <Form.Control
                type="text"
                placeholder="Type a message"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                onKeyPress={(e) => {
                  if (e.key === 'Enter') {
                    sendMessage();
                  }
                }}
              />
            </Col>
          </Row>

          <Row className="my-4">
            <Col sm="12">
              <Button variant="success" onClick={sendMessage}>
                Send Message
              </Button>
              <Button variant="info" onClick={fetchMessages} className="ml-2">
                Fetch Messages
              </Button>
            </Col>
          </Row>
        </>
      )}
    </Container>
  );
};

export default Chat;
