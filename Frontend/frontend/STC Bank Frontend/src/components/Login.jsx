import React, { useState, useEffect } from "react";
import Image from "../assets/image.png";
import Logo from "../assets/logo.png";
import GoogleSvg from "../assets/icons8-google.svg";
import { FaEye, FaEyeSlash } from "react-icons/fa6";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { Link } from "react-router-dom";
import './index.css';
import './responsive.css';
import axios from "axios";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { GoogleLogin } from "react-google-login";
import { useAuth } from '../../Contexts/AuthContext'









const Login = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showOTPModal, setShowOTPModal] = useState(false);
  const [otp, setOtp] = useState("");
  const [isVerifying, setIsVerifying] = useState(false);
  const { setUserData } = useAuth();
  const clientId = "837942369513-ka2sp66d2fepp9ida4p05ls23nt9rvr5.apps.googleusercontent.com";

  useEffect(() => {
    const loadFacebookSDK = () => {
      if (window.FB) {
        window.FB.init({
          appId: '9092235867539242',
          cookie: true,
          xfbml: true,
          version: 'v12.0',
        });
        console.log("Facebook SDK initialized successfully.");
      } else {
        console.error("Facebook SDK initialization failed.");
      }
    };

    (function (d, s, id) {
      let js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) return;
      js = d.createElement(s);
      js.id = id;
      js.src = 'https://connect.facebook.net/en_US/sdk.js';
      fjs.parentNode.insertBefore(js, fjs);

      js.onload = loadFacebookSDK;
    })(document, 'script', 'facebook-jssdk');
  }, []);

  const handleLogin = async () => {
    if (!email || !password) {
      toast.warn("Please fill in both fields");
      return;
    }

    try {
      const response = await axios.post(
        "https://localhost:7152/api/Login/Login",
        { email, password },
        { headers: { "Content-Type": "application/json" } }
      );

      toast.success("OTP sent to your registered phone number!");
      setShowOTPModal(true);
    } catch (error) {
      if (error.response) {
        const errorMessage = error.response.data?.error || 
                         error.response.data?.message || 
                         "Login failed. Please try again.";
        toast.error(errorMessage);
        console.error("Login error:", error.response.data);
      } else {
        toast.error("Network error. Please check your connection.");
        console.error("Login failed:", error.message);
      }
    }
  };

  const handleFacebookLogin = () => {
    if (window.FB) {
      window.FB.login(
        (response) => {
          if (response.authResponse) {
            console.log("Facebook login successful:", response);
            const { accessToken, userID } = response.authResponse;

            window.FB.api(
              "/me",
              { fields: "name,email" },
              async (userInfo) => {
                console.log("Facebook user info:", userInfo);

                try {
                  toast.loading("Logging in with Facebook, please wait...");
                  const result = await axios.post(
                    "https://localhost:7152/api/Login/FacebookLogin",
                    { FacebookId: userID }
                  );
                  toast.dismiss();
                  toast.success("OTP sent to your registered phone number!");
                  setShowOTPModal(true);
                } catch (error) {
                  console.error("Error during Facebook login:", error);
                  toast.dismiss();
                  
                  if (error.response) {
                    const errorMessage = "Signup first, you don't have account";
                    toast.error(errorMessage);
                  } else {
                    toast.error("Network error. Please check your connection.");
                  }
                }
              }
            );
          } else {
            console.error("Facebook login failed:", response);
            toast.error("Facebook login cancelled or failed. Please try again.");
          }
        },
        { scope: "email" }
      );
    } else {
      console.error("Facebook SDK not loaded.");
      toast.error("Facebook SDK not loaded, please try again later.");
    }
  };

  const handleGoogleSuccess = async (response) => {
    const user = response.profileObj;
    
    try {
      toast.loading("Logging in with Google, please wait...");
      const result = await axios.post(
        "https://localhost:7152/api/Login/GoogleLogin",
        { gmail: user.email }
      );
      toast.dismiss();
      toast.success("OTP sent to your registered phone number!");
      setShowOTPModal(true);
    } catch (error) {
      console.error("Error during Google login:", error);
      toast.dismiss();
      
      if (error.response) {
        const errorMessage = "Signup first you don't have an account";
        toast.error(errorMessage);
      } else {
        toast.error("Network error. Please check your connection.");
      }
    }
  };

  const handleGoogleFailure = (error) => {
    console.error("Google sign-in failed:", error);
    toast.error("Google sign-in failed or was cancelled. Please try again.");
  };




const handleVerifyOTP = async () => {
  if (!otp) {
    toast.warn("Please enter the OTP");
    return;
  }

  setIsVerifying(true);
  try {
    const response = await axios.post(
      "https://localhost:7152/api/Login/VerifyOtp",
      { 
        otp: otp,
        key: "default" // Using the default key as specified
      },
      { headers: { "Content-Type": "application/json" } }
    );

    // Extract userId and username from the response AFTER the API call succeeds
    const { userId, username } = response.data;

    // Now you can use them safely
    toast.success(`Welcome ${username}! (ID: ${userId})`);
    
    // Store userId in localStorage
    localStorage.setItem("userId", userId);

    setUserData(userId, username);

    setShowOTPModal(false);
  } catch (error) {
    if (error.response) {
      const errorMessage = error.response.data?.error || 
                       error.response.data?.message || 
                       "OTP verification failed";
      toast.error(errorMessage);
    } else {
      toast.error("Network error. Please try again.");
    }
  } finally {
    setIsVerifying(false);
  }
};


  return (
    <div className="login-main">
      <ToastContainer position="top-right" autoClose={5000} />
      
      {/* OTP Modal */}
      {showOTPModal && (
        <div className="otp-modal-overlay">
          <div className="otp-modal-container">
            <div className="otp-modal-header">
              <h3>Verify Your Identity</h3>
              <button 
                onClick={() => setShowOTPModal(false)} 
                className="otp-modal-close"
              >
                &times;
              </button>
            </div>
            
            <div className="otp-modal-body">
              <p>We've sent a 6-digit OTP to your registered phone number.</p>
              <p>Please enter it below to complete your login.</p>
              
              <div className="otp-input-container">
                <input
                  type="text"
                  value={otp}
                  onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
                  maxLength={6}
                  className="otp-input"
                />
              </div>
            </div>
            
            <div className="otp-modal-footer">
              <button 
                onClick={handleVerifyOTP} 
                disabled={isVerifying}
                className="otp-verify-btn"
              >
                {isVerifying ? "Verifying..." : "Verify OTP"}
              </button>
              

            </div>
          </div>
        </div>
      )}

      <div className="login-left">
        <img src={Image} alt="Background" />
      </div>
      <div className="login-right">
        <div className="login-right-container">
          <div className="login-logo">
            <img src={Logo} alt="Logo" />
            <h1 className="stc-logo ms-2">STC</h1>
          </div>
          <div className="login-center">
            <h2>Welcome back!</h2>
            <p>Please enter your details</p>
            <form onSubmit={(e) => e.preventDefault()}>
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />

              <div className="pass-input-div">
                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="Password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                {showPassword ? (
                  <FaEyeSlash onClick={() => setShowPassword(false)} />
                ) : (
                  <FaEye onClick={() => setShowPassword(true)} />
                )}
              </div>

              <div className="login-center-options">
                <a href="#" className="forgot-pass-link">
                  Forgot password?
                </a>
              </div>

              <div className="login-center-buttons">
                <button type="button" onClick={handleLogin}>
                  Log In
                </button>

                <GoogleLogin
                  clientId={clientId}
                  render={(renderProps) => (
                    <button 
                      type="button" 
                      className="google-login-btn"
                      onClick={renderProps.onClick}
                      disabled={renderProps.disabled}
                    >
                      <img src={GoogleSvg} alt="Google Icon" />
                      Log In with Google
                    </button>
                  )}
                  onSuccess={handleGoogleSuccess}
                  onFailure={handleGoogleFailure}
                  cookiePolicy="single_host_origin"
                />

                <button 
                  type="button" 
                  className="btn btn-primary facebook-login-btn"
                  onClick={handleFacebookLogin}
                >
                  <FontAwesomeIcon icon={faFacebook} className="me-3 mb-2 facebook-icon" />
                  Log In with Facebook
                </button>
              </div>
            </form>
          </div>

          <p className="login-bottom-p">
            Don't have an account? <Link to="/">Sign Up</Link>
          </p>
        </div>
      </div>

    </div>
  );
};

export default Login;