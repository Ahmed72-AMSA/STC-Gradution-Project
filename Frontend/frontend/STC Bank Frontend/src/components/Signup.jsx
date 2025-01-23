import React, { useState } from "react";
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
    email: "",
    password: "",
    userName: "",
    phoneNumber: "",
    nationalId: "",
  });


  

  const [userId , setUserId] = useState(0)

  const [National_ID_Google, Set_National_ID_Google] = useState({
  nationalIdGoog: "",
  });

  const [modalVisible, setModalVisible] = useState(false); // State for modal visibility
  const [googleUser, setGoogleUser] = useState(null); // State to store Google user data



  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  // Validation methods
  const isValidEmail = (email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const isValidPassword = (password) => /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$/.test(password);
  const isValidPhoneNumber = (phoneNumber) => /^\d{11}$/.test(phoneNumber);
  const isValidNationalId = (nationalId) => /^\d{14}$/.test(nationalId);

  
  
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

      const result = await axios.post("http://localhost:5001/api/SignUp/Google", googleSignUpRequest);
      console.log("User signed up successfully:", result.data);

      toast.dismiss();
      toast.success("User signed up successfully!", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      setUserId(result.data.id)

      setGoogleUser(user); 
      setModalVisible(true); 
    } catch (error) {
      console.error("Error during Google sign-up:", error);
      toast.dismiss();
      toast.error("Error during Google sign-up, please try again.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };

  // Failure handler for Google login
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
    if (!formData.password || !isValidPassword(formData.password)) {
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
    if (!formData.nationalId || !isValidNationalId(formData.nationalId)) {
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

      const result = await axios.post("http://localhost:5001/api/SignUp", formData);
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




  const handleNationalIdSubmit = async () => {
    if (!National_ID_Google.nationalIdGoog) {
      toast.error("Please enter a valid National ID.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      return;
    }
  
    // Update National ID after validation
    const updateNationalIdRequest = {
      nationalId: National_ID_Google.nationalIdGoog,
    };

    try {
      await axios.put(`http://localhost:5001/api/SignUp/Google/NationalId/${userId}`, updateNationalIdRequest);
      toast.success("National ID updated successfully!", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
      setModalVisible(false); // Close the modal after successful submission
    } catch (error) {
      console.error("Error during National ID update:", error);
      toast.error("Error during National ID update, please try again.", {
        position: "top-right",
        autoClose: 5000,
        theme: "colored",
      });
    }
  };



  

  const handleModalClose = async () => {
      try {
        await axios.delete(`http://localhost:5001/api/SignUp/${userId}`);
        toast.success("You has been deleted due to missing National ID", {
          position: "top-right",
          autoClose: 5000,
          theme: "colored",
        });
      } catch (error) {
        console.error("Error during user deletion:", error);
        toast.error("Error during user deletion.", {
          position: "top-right",
          autoClose: 5000,
          theme: "colored",
        });
      }
      setModalVisible(false);

    }



  return (
    <div className="signup-main">
      <div className="signup-container">
        <div className="login-logo mb-5">
          <img src={Logo} alt="Logo" />
          <h1 className="">STC</h1>
        </div>

        {/* Local signup form */}
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
            name="password"
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
            name="nationalId"
            value={formData.nationalId}
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
            <button className="facebook-btn">
              <FontAwesomeIcon icon={faFacebook} />
            </button>
          </div>
        </form>

        <p className="signup-register">
          Already have an account? <Link to="/login">Login Now!</Link>
        </p>
      </div>

      {/* Modal */}
      {/* Modal */}


      {modalVisible && (
      <div className="modal fade show" tabIndex="-1" role="dialog">
        <div className="modal-dialog" role="document">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title">National-ID needed to complete registration</h5>
            </div>
            <div className="modal-body">
              <input
                type="text"
                name="nationalIdGoog"
                value={National_ID_Google.nationalIdGoog}
                onChange={(e) => Set_National_ID_Google({ ...National_ID_Google, nationalIdGoog: e.target.value })}
                placeholder="Enter your National ID"
                className="signup-form-input"
              />
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" onClick={handleModalClose}>
                Close
              </button>
              <button type="button" className="btn btn-primary" onClick={handleNationalIdSubmit}>
                Submit
              </button>
            </div>
          </div>
        </div>
      </div>
    )}




      <ToastContainer />
    </div>
  );
};

export default Signup; 