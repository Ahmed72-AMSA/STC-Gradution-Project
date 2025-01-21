import React, { useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { gapi } from 'gapi-script';
import Signup from './components/Signup';
import Login from './components/Login';

function App() {
  const clientId = '837942369513-ka2sp66d2fepp9ida4p05ls23nt9rvr5.apps.googleusercontent.com';

  useEffect(() => {
    // Initialize the Google API client
    const initClient = () => {
      gapi.client.init({
        clientId: clientId,
        scope: '',
      });
    };

    gapi.load('client:auth2', initClient); // Load auth2 library and initialize
  }, [clientId]);

  return (
    <Router>
      <Routes>
      <Route path="/" element={<Signup />} />
      <Route path="/login" element={<Login />} />
      </Routes>
    </Router>
  );
}

export default App;
