import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFacebook, faGithub } from "@fortawesome/free-brands-svg-icons";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import GoogleSvg from "../assets/icons8-google.svg";
import Logo from "../assets/logo.png";
import { Link } from "react-router-dom";


import "./Signup.css";
import "./responsive.css";

const Signup = () => {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="signup-main">
      <div className="signup-container">
        <div className="login-logo mb-5">
          <img src={Logo} alt="Logo" />
          <h1 className="">STC</h1>
        </div>

        <form className="signup-form">
          <input type="email" placeholder="Email" />
            <input
              type={showPassword ? "text" : "password"}
              placeholder="Password"
            />
    
          <input type="text" placeholder="Name" />
          <input type="tel" placeholder="Phone Number" />

          <button type="button" className="btn signup-btn">
            Sign up
          </button>
          <p className="signup-or">or continue with</p>
          <div className="signup-social">
            <button className="google-btn">
              <img src={GoogleSvg} alt="Google" />
            </button>
            <button className="facebook-btn">
              <FontAwesomeIcon icon={faFacebook} />
            </button>
     
          </div>
        </form>
        <p className="signup-register">
          Already have an account ? <Link to="/login" >Login Now !</Link>
        </p>
      </div>
    </div>
  );
};

export default Signup;
