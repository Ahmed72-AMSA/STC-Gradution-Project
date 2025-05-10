import React, { useState, useRef, useEffect } from 'react';
import './Chatbot.css'; // You'll need to adapt your CSS for React

const Chatbot = () => {



    useEffect(() => {
        // Create link element
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = 'https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@48,400,0,0&family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@48,400,1,0';
        
        // Add to document head
        document.head.appendChild(link);
        
        // Cleanup on unmount
        return () => {
          document.head.removeChild(link);
        };
      }, []);



  const [message, setMessage] = useState('');
  const [messages, setMessages] = useState([
    {
      type: 'bot',
      content: 'hey, there<br>How can I help you today?',
      attachment: null
    }
  ]);
  const [file, setFile] = useState(null);
  const [filePreview, setFilePreview] = useState(null);
  const [isChatbotOpen, setIsChatbotOpen] = useState(false);
  const messageInputRef = useRef(null);
  const chatBodyRef = useRef(null);

  const API_KEY = "AIzaSyAB9YvaA2DiueTisWCdR3vrq5rzAEEMlfg";
  const API_URL = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${API_KEY}`;

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!message.trim() && !file) return;

    // Add user message
    const newUserMessage = {
      type: 'user',
      content: message,
      attachment: filePreview
    };
    setMessages(prev => [...prev, newUserMessage]);
    
    // Add thinking indicator
    const thinkingMessage = {
      type: 'bot',
      content: '',
      isThinking: true
    };
    setMessages(prev => [...prev, thinkingMessage]);

    // Clear inputs
    setMessage('');
    setFile(null);
    setFilePreview(null);

    // Generate bot response
    await generateBotResponse(message, file);
  };

  const generateBotResponse = async (userMessage, userFile) => {
    const requestOptions = {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        contents: [{
          parts: [
            { text: userMessage },
            ...(userFile ? [{ inline_data: userFile }] : [])
          ]
        }]
      })
    };

    try {
      const response = await fetch(API_URL, requestOptions);
      const data = await response.json();
      
      if (!response.ok) throw new Error(data.error?.message || 'API request failed');

      const apiResponseText = data.candidates[0].content.parts[0].text
        .replace(/\*\*(.*?)\*\*/g, "$1")
        .trim();

      // Replace thinking message with actual response
      setMessages(prev => {
        const newMessages = [...prev];
        newMessages[newMessages.length - 1] = {
          type: 'bot',
          content: apiResponseText,
          isThinking: false
        };
        return newMessages;
      });
    } catch (error) {
      console.error(error);
      // Update thinking message with error
      setMessages(prev => {
        const newMessages = [...prev];
        newMessages[newMessages.length - 1] = {
          type: 'bot',
          content: error.message,
          isError: true,
          isThinking: false
        };
        return newMessages;
      });
    }
  };

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (!selectedFile) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      setFilePreview(e.target.result);
      
      const base64String = e.target.result.split(",")[1];
      setFile({
        data: base64String,
        mime_type: selectedFile.type
      });
    };
    reader.readAsDataURL(selectedFile);
  };

  const removeFile = () => {
    setFile(null);
    setFilePreview(null);
  };

  useEffect(() => {
    // Auto-scroll to bottom when messages change
    if (chatBodyRef.current) {
      chatBodyRef.current.scrollTop = chatBodyRef.current.scrollHeight;
    }
  }, [messages]);

  return (
    <div className={`chatbot-container ${isChatbotOpen ? 'show-chatbot' : ''}`}>
      <button 
        className="chatbot-toggler"
        onClick={() => setIsChatbotOpen(!isChatbotOpen)}
      >
        <span className="material-symbols-rounded">mode_comment</span>
        <span className="material-symbols-rounded">close</span>
      </button>

      <div className="chatbot-popup">
        <div className="chat-header">
          <div className="header-info">
            <svg className="chatbot-logo" xmlns="http://www.w3.org/2000/svg" width="50" height="50" viewBox="0 0 1024 1024">
              <path d="M738.3 287.6H285.7c-59 0-106.8 47.8-106.8 106.8v303.1c0 59 47.8 106.8 106.8 106.8h81.5v111.1c0 .7.8 1.1 1.4.7l166.9-110.6 41.8-.8h117.4l43.6-.4c59 0 106.8-47.8 106.8-106.8V394.5c0-59-47.8-106.9-106.8-106.9zM351.7 448.2c0-29.5 23.9-53.5 53.5-53.5s53.5 23.9 53.5 53.5-23.9 53.5-53.5 53.5-53.5-23.9-53.5-53.5zm157.9 267.1c-67.8 0-123.8-47.5-132.3-109h264.6c-8.6 61.5-64.5 109-132.3 109zm110-213.7c-29.5 0-53.5-23.9-53.5-53.5s23.9-53.5 53.5-53.5 53.5 23.9 53.5 53.5-23.9 53.5-53.5 53.5zM867.2 644.5V453.1h26.5c19.4 0 35.1 15.7 35.1 35.1v121.1c0 19.4-15.7 35.1-35.1 35.1h-26.5zM95.2 609.4V488.2c0-19.4 15.7-35.1 35.1-35.1h26.5v191.3h-26.5c-19.4 0-35.1-15.7-35.1-35.1zM561.5 149.6c0 23.4-15.6 43.3-36.9 49.7v44.9h-30v-44.9c-21.4-6.5-36.9-26.3-36.9-49.7 0-28.6 23.3-51.9 51.9-51.9s51.9 23.3 51.9 51.9z"></path>
            </svg>
            <h2 className="logo-text">ChatBot</h2>
          </div>
          <button 
            className="close-chatbot material-symbols-rounded"
            onClick={() => setIsChatbotOpen(false)}
          >
            keyboard_arrow_down
          </button>
        </div>

        <div className="chat-body" ref={chatBodyRef}>
          {messages.map((msg, index) => (
            <div key={index} className={`message ${msg.type}-message ${msg.isError ? 'error' : ''}`}>
              {msg.type === 'bot' && (
                <svg className="bot-avatar" xmlns="http://www.w3.org/2000/svg" width="50" height="50" viewBox="0 0 1024 1024">
                  <path d="M738.3 287.6H285.7c-59 0-106.8 47.8-106.8 106.8v303.1c0 59 47.8 106.8 106.8 106.8h81.5v111.1c0 .7.8 1.1 1.4.7l166.9-110.6 41.8-.8h117.4l43.6-.4c59 0 106.8-47.8 106.8-106.8V394.5c0-59-47.8-106.9-106.8-106.9zM351.7 448.2c0-29.5 23.9-53.5 53.5-53.5s53.5 23.9 53.5 53.5-23.9 53.5-53.5 53.5-53.5-23.9-53.5-53.5zm157.9 267.1c-67.8 0-123.8-47.5-132.3-109h264.6c-8.6 61.5-64.5 109-132.3 109zm110-213.7c-29.5 0-53.5-23.9-53.5-53.5s23.9-53.5 53.5-53.5 53.5 23.9 53.5 53.5-23.9 53.5-53.5 53.5zM867.2 644.5V453.1h26.5c19.4 0 35.1 15.7 35.1 35.1v121.1c0 19.4-15.7 35.1-35.1 35.1h-26.5zM95.2 609.4V488.2c0-19.4 15.7-35.1 35.1-35.1h26.5v191.3h-26.5c-19.4 0-35.1-15.7-35.1-35.1zM561.5 149.6c0 23.4-15.6 43.3-36.9 49.7v44.9h-30v-44.9c-21.4-6.5-36.9-26.3-36.9-49.7 0-28.6 23.3-51.9 51.9-51.9s51.9 23.3 51.9 51.9z"></path>
                </svg>
              )}
              
              <div className="message-text">
                {msg.isThinking ? (
                  <div className="thinking-indicator">
                    <div className="dot"></div>
                    <div className="dot"></div>
                    <div className="dot"></div>
                  </div>
                ) : (
                  <>
                    {msg.content && <div dangerouslySetInnerHTML={{ __html: msg.content }} />}
                    {msg.attachment && <img src={msg.attachment} className="attachment" alt="User attachment" />}
                  </>
                )}
              </div>
            </div>
          ))}
        </div>

        <div className="chat-footer">
          <form onSubmit={handleSubmit} className="chat-form">
            <textarea
              ref={messageInputRef}
              placeholder="message..."
              className="message-input"
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !e.shiftKey && window.innerWidth > 768) {
                  handleSubmit(e);
                }
              }}
              required
            />
            <div className="chat-controls">
              <button type="button" className="material-symbols-outlined">sentiment_satisfied</button>
              <div className={`file-upload-wrapper ${filePreview ? 'file-uploaded' : ''}`}>
                <input 
                  type="file" 
                  accept="image/*" 
                  id="file-input" 
                  onChange={handleFileChange}
                  hidden
                />
                {filePreview && <img src={filePreview} alt="Preview" />}
                <button 
                  type="button" 
                  id="file-upload" 
                  className="material-symbols-rounded"
                  onClick={() => document.getElementById('file-input').click()}
                >
                  attach_file
                </button>
                {filePreview && (
                  <button 
                    type="button" 
                    id="file-cancel" 
                    className="material-symbols-rounded"
                    onClick={removeFile}
                  >
                    close
                  </button>
                )}
              </div>
              <button type="submit" className="material-symbols-rounded">arrow_upward</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Chatbot;