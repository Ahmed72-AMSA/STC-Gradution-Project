import React, { useState } from "react";
import Image from "../assets/image.png";
import Logo from "../assets/logo.png";
import GoogleSvg from "../assets/icons8-google.svg";
import { FaEye, FaEyeSlash } from "react-icons/fa6";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { Link } from "react-router-dom";
import './index.css';
import './responsive.css';

const Login = () => {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="login-main">
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
            <form>
              <input type="email" placeholder="Email" />
              <div className="pass-input-div">
                <input 
                  type={showPassword ? "text" : "password"} 
                  placeholder="Password" 
                />
                {showPassword ? (
                  <FaEyeSlash onClick={() => setShowPassword(!showPassword)} />
                ) : (
                  <FaEye onClick={() => setShowPassword(!showPassword)} />
                )}
              </div>

              <div className="login-center-options">
                <a href="#" className="forgot-pass-link">
                  Forgot password?
                </a>
              </div>
              <div className="login-center-buttons">
                <button type="button">Log In</button>
                <button type="button" className="google-login-btn">
                  <img src={GoogleSvg} alt="Google Icon" />
                  Log In with Google
                </button>
                <button type="button" className="btn btn-primary facebook-login-btn">
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
