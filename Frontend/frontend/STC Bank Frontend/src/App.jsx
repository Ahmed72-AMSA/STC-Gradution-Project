import React, { useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { gapi } from 'gapi-script';

// Auth Context
import { AuthProvider } from '../Contexts/AuthContext';

// Main App Components
import Signup from './components/Signup';
import Login from './components/Login';
import Chat from './components/Chat';
import CreditForm from './components/CreditForm';
import Home from './components/Home';
import Profile from './components/ProfileIndex';

// Dashboard Components
import Sidebar from './components/Sidebar.jsx';
import Users from './components/UsersManagment.jsx';
import DashboardNavbar from './components/navbar.jsx';

function App() {
  const clientId = '837942369513-ka2sp66d2fepp9ida4p05ls23nt9rvr5.apps.googleusercontent.com';

  useEffect(() => {
    const initClient = () => {
      gapi.client.init({
        clientId: clientId,
        scope: '',
      });
    };
    gapi.load('client:auth2', initClient);
  }, [clientId]);

  return (
    <AuthProvider>
      <Router>
        <Routes>
          {/* Public/Main Routes */}
          <Route path="/" element={<Signup />} />
          <Route path="/login" element={<Login />} />
          <Route path="/chat" element={<Chat />} />
          <Route path="/loan" element={<CreditForm />} />
          <Route path="/home" element={<Home />} />
          <Route path="/profile" element={<Profile />} />

          {/* Dashboard Routes with Nested Layout */}
          <Route
            path="/dashboard/*"
            element={
              <>
                <DashboardNavbar />
                <Sidebar>
                  <Routes>
                    <Route path="Users" element={<Users/>} />
                  </Routes>
                </Sidebar>
              </>
            }
          />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
