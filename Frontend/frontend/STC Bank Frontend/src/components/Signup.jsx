import React, { useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import GoogleSvg from "../assets/icons8-google.svg"; 
import Logo from "../assets/logo.png";
import { Link } from "react-router-dom";
import axios from "axios";
import { GoogleLogin } from "react-google-login";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css"; 
import "./index.css"
import "./Signup.css";
import "./responsive.css";

const Signup = () => {
  const clientId = "837942369513-ka2sp66d2fepp9ida4p05ls23nt9rvr5.apps.googleusercontent.com"; 
  const [showPassword, setShowPassword] = useState(false);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    hashedPassword: "",
    role:"Unknown",
    phoneNumber: "",
    nationalI: ""
  });



  const [googleModalVisible, setGoogleModalVisible] = useState(false);
  const [googleAdditionalData, setGoogleAdditionalData] = useState({
    phoneNumber: "",
    nationalID: ""
  });

  const [userId, setUserId] = useState(0);
  const [modalVisible, setModalVisible] = useState(false);
  const [googleUser, setGoogleUser] = useState(null);
  const [facebookModalVisible, setFacebookModalVisible] = useState(false);
  const [facebookUser, setFacebookUser] = useState(null);
  const [facebookAdditionalData, setFacebookAdditionalData] = useState({
    phoneNumber: "",
    nationalID: ""
  });
  const [National_ID_Google, Set_National_ID_Google] = useState({
    nationalIdGoog: "",
  });

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

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  // Validation methods
  const isValidEmail = (email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const isValidPassword = (password) => /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$/.test(password);
  const isValidPhoneNumber = (phoneNumber) => /^\d{11}$/.test(phoneNumber);
  const isValidNationalId = (nationalId) => /^\d{14}$/.test(nationalId);

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
                  toast.loading("Signing up with Facebook, please wait...", {
                    position: "top-right",
                    theme: "colored",
                    autoClose: false,
                  });

                  const result = await axios.post(
                    "https://localhost:7152/api/Users/facebook",
                    {
                      FacebookId: userID,
                      UserName: userInfo.name,
                      Email: userInfo.email,
                      role: "Unknown"
                    }
                  );

                  console.log("User signed up successfully:", result.data);
                  setUserId(result.data.id);
                  setFacebookUser({
                    id: userID,
                    name: userInfo.name,
                    email: userInfo.email
                  });
                  setFacebookModalVisible(true);

                  toast.dismiss();
                } catch (error) {
                  console.error("Error during Facebook sign-up:", error);
                  toast.dismiss();
                  toast.error(
                     "Facebook sign-up failed this account is already registered",
                    {
                      position: "top-right",
                      autoClose: 5000,
                      theme: "colored",
                    }
                  );
                }
              }
            );
          } else {
            console.error("Facebook login failed:", response);
            toast.error("Facebook sign-up failed. Please try again.", {
              position: "top-right",
              autoClose: 5000,
              theme: "colored",
            });
          }
        },
        { scope: "email" }
      );
    } else {
      console.error("Facebook SDK not loaded.");
      toast.error("Facebook SDK not loaded, please try again later.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };

  const handleFacebookAdditionalDataSubmit = async () => {
    if (!facebookAdditionalData.phoneNumber || !isValidPhoneNumber(facebookAdditionalData.phoneNumber)) {
      toast.error("Phone number must be exactly 11 digits.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }

    if (!facebookAdditionalData.nationalID || !isValidNationalId(facebookAdditionalData.nationalID)) {
      toast.error("National ID must be exactly 14 digits.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }

    try {
      const updateData = {
        facebookId: facebookUser.id,
        phoneNumber: facebookAdditionalData.phoneNumber,
        nationalID: facebookAdditionalData.nationalID
      };

      await axios.patch("https://localhost:7152/api/Users/facebook/update-contact", updateData);
      
      toast.success("Registration completed successfully!", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      
      setFacebookModalVisible(false);
    } catch (error) {
      console.error("Error completing Facebook registration:", error);
      toast.error(error.response?.data || "Error completing registration", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };

  const handleFacebookModalClose = async () => {
    try {
        await axios.delete(`https://localhost:7152/api/Users/facebook?facebookId=${facebookUser.id}`);      
         toast.info("Registration cancelled. Your account has been removed.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    } catch (error) {
      console.error("Error cancelling Facebook registration:", error);
      toast.error("Error cancelling registration and you not signed-up", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
    setFacebookModalVisible(false);
  };









const handleGoogleSuccess = async (response) => {
    const user = response.profileObj;
    const googleSignUpRequest = {
      userName: user.name,
      email: user.email,
      gmail: user.email,
    };

    try {
      toast.loading("Signing up with Google, please wait...", {
        position: "top-right",
        theme: "colored",
        autoClose: false,
      });

      const result = await axios.post("https://localhost:7152/api/Users/google", googleSignUpRequest);
      console.log("User signed up successfully:", result.data);

      toast.dismiss();
      toast.success("User signed up successfully! Please complete your registration.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      
      setUserId(result.data.userId);
      
      setGoogleUser(user);
      setGoogleModalVisible(true); 

    } catch (error) {
      console.error("Error during Google sign-up:", error);
      toast.dismiss();
      toast.error(error.response?.data || "Google sign-up failed", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };
  console.log("hi"+ userId)

  const handleGoogleAdditionalDataSubmit = async () => {
    if (!googleAdditionalData.phoneNumber || !isValidPhoneNumber(googleAdditionalData.phoneNumber)) {
      toast.error("Phone number must be exactly 11 digits.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }

    if (!googleAdditionalData.nationalID || !isValidNationalId(googleAdditionalData.nationalID)) {
      toast.error("National ID must be exactly 14 digits.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }

    try {
      const updateData = {
        phoneNumber: googleAdditionalData.phoneNumber,
        nationalID: googleAdditionalData.nationalID
      };

      await axios.patch(`https://localhost:7152/api/Users/google/update-contact/${userId}`, updateData);
      
      toast.success("Registration completed successfully!", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      
      setGoogleModalVisible(false);
    } catch (error) {
      console.error("Error completing Google registration:", error);
      toast.error(error.response?.data || "Error completing registration", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };

  const handleGoogleModalClose = async () => {
    try {
      await axios.delete(`https://localhost:7152/api/Users/${userId}`);
      toast.info("Registration cancelled. Your account has been removed.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    } catch (error) {
      console.error("Error cancelling Google registration:", error);
      toast.error("Error cancelling registration and you are not signed-up", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
    setGoogleModalVisible(false);
  };

  const handleGoogleFailure = (error) => {
    console.error("Google sign-in failed:", error);
    toast.error("Google sign-in failed, please try again.", {
      position: "top-right",
      autoClose: 5000,
      theme: "colored",
    });
  };















  const handleFormSubmit = async (e) => {
    e.preventDefault();

    if (!formData.userName) {
      toast.error("User Name is required.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }
    if (!formData.email || !isValidEmail(formData.email)) {
      toast.error("Valid Email is required.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }
    if (!formData.hashedPassword || !isValidPassword(formData.hashedPassword)) {
      toast.error("Password must be at least 6 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one non-alphanumeric character.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }
    if (!formData.phoneNumber || !isValidPhoneNumber(formData.phoneNumber)) {
      toast.error("Phone number must be exactly 11 digits.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }
    if (!formData.nationalID || !isValidNationalId(formData.nationalID)) {
      toast.error("National ID must be exactly 14 digits.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }

    try {
      toast.loading("Signing up, please wait...", {
        position: "top-right",
        theme: "colored",
        autoClose: false,
      });

    const signupData = {
    userName: formData.userName,
    email: formData.email,
    hashedPassword: formData.hashedPassword,
    role: formData.role,
    phoneNumber: formData.phoneNumber,
    nationalID: formData.nationalID
  };



      const result = await axios.post("https://localhost:7152/api/Users/register", signupData);
      console.log("User signed up successfully:", result.data);

      toast.dismiss();
      toast.success("User signed up successfully!", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    } catch (error) {
      console.error("Error during form-based sign-up:", error);

      toast.dismiss();
      toast.error("Error during form-based sign-up, please check another username, email, or National ID as it's already taken.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };

  

  return (
    <div>
      <ToastContainer
        position="top-right"
        autoClose={5000}
        theme="colored"
        style={{ zIndex: 9999 }}
      />

      {/* Facebook Modal */}
      {facebookModalVisible && (
        <div className="facebook-modal-overlay">
          <div className="facebook-modal">
            <h2>Complete Your Facebook Registration</h2>
            <p>Please provide your phone number and national ID to complete registration</p>
            
            <div className="facebook-modal-input-group">
              <label>Phone Number</label>
              <input
                type="text"
                value={facebookAdditionalData.phoneNumber}
                onChange={(e) => setFacebookAdditionalData({
                  ...facebookAdditionalData,
                  phoneNumber: e.target.value
                })}
                placeholder="11-digit phone number"
              />
            </div>
            
            <div className="facebook-modal-input-group">
              <label>National ID</label>
              <input
                type="text"
                value={facebookAdditionalData.nationalID}
                onChange={(e) => setFacebookAdditionalData({
                  ...facebookAdditionalData,
                  nationalID: e.target.value
                })}
                placeholder="14-digit national ID"
              />
            </div>
            
            <div className="facebook-modal-buttons">
              <button 
                className="facebook-modal-submit"
                onClick={handleFacebookAdditionalDataSubmit}
              >
                Submit
              </button>
              <button 
                className="facebook-modal-cancel"
                onClick={handleFacebookModalClose}
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

 {googleModalVisible && (
        <div className="google-modal-overlay">
          <div className="google-modal">
            <h2>Complete Your Google Registration</h2>
            <p>Please provide your phone number and national ID to complete registration</p>
            
            <div className="google-modal-input-group">
              <label>Phone Number</label>
              <input
                type="text"
                value={googleAdditionalData.phoneNumber}
                onChange={(e) => setGoogleAdditionalData({
                  ...googleAdditionalData,
                  phoneNumber: e.target.value
                })}
                placeholder="11-digit phone number"
              />
            </div>
            
            <div className="google-modal-input-group">
              <label>National ID</label>
              <input
                type="text"
                value={googleAdditionalData.nationalID}
                onChange={(e) => setGoogleAdditionalData({
                  ...googleAdditionalData,
                  nationalID: e.target.value
                })}
                placeholder="14-digit national ID"
              />
            </div>
            
            <div className="google-modal-buttons">
              <button 
                className="google-modal-submit"
                onClick={handleGoogleAdditionalDataSubmit}
              >
                Submit
              </button>
              <button 
                className="google-modal-cancel"
                onClick={handleGoogleModalClose}
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="signup-main">
        <div className="signup-container">
          <div className="login-logo mb-5">
            <img src={Logo} alt="Logo" />
            <h1 className="">STC</h1>
          </div>

          <form className="signup-form" onSubmit={handleFormSubmit}>
            <input
              type="text"
              name="userName"
              value={formData.userName}
              onChange={handleInputChange}
              placeholder="UserName"
              className="signup-form-input"
            />
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              placeholder="Email"
              className="signup-form-input"
            />
            <input
              type={showPassword ? "text" : "password"}
              name="hashedPassword"
              value={formData.password}
              onChange={handleInputChange}
              placeholder="Password"
              className="signup-form-input"
            />
            <input
              type="text"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleInputChange}
              placeholder="Phone Number"
              className="signup-form-input"
            />
            <input
              type="text"
              name="nationalID"
              value={formData.nationalID}
              onChange={handleInputChange}
              placeholder="National ID"
              className="signup-form-input"
            />

            <button type="submit" className="btn signup-btn">
              Sign up
            </button>

            <p className="signup-or">or continue with</p>
            <div className="signup-social">
              <GoogleLogin
                clientId={clientId}
                render={(renderProps) => (
                  <button
                    className="google-btn"
                    onClick={renderProps.onClick}
                    disabled={renderProps.disabled}
                  >
                    <img src={GoogleSvg} alt="Google" className="google-icon" />
                  </button>
                )}
                onSuccess={handleGoogleSuccess}
                onFailure={handleGoogleFailure}
                cookiePolicy="single_host_origin"
              />

              <button
                onClick={handleFacebookLogin}
              >
                <FontAwesomeIcon icon={faFacebook} className="facebook-icon" />
              </button>
            </div>

            <p className="signup-register">
              Already have an account? <Link to="/login">Login Now!</Link>
            </p>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Signup;