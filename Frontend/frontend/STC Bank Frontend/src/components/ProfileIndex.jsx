import React, { useState, useRef, useEffect } from "react";
import { 
  FiCamera, FiEdit, FiCast, FiTrash2, FiLogOut, 
  FiUser, FiMail, FiPhone, FiMapPin, FiLinkedin, 
  FiFacebook, FiBriefcase, FiDollarSign, FiCheckCircle, FiX, FiSend,
  FiClock, FiCheck,FiPlus
} from "react-icons/fi";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./profile.css";
import { Link } from 'react-router-dom';
import axios from 'axios';
import Modal from 'react-modal';
import userImage from '../images/userprofile.jpg';

Modal.setAppElement('#root');

const Profile = () => {
  const [userData, setUserData] = useState({
    general: {
      userId: "",
      userName: "",
      email: "",
      balance: 0
    },
    Deposit: {
      accountId: "",
      amount: ""
    },
    Withdrawal: {
      accountId: "",
      amount: ""
    },
    Transfer: {
      fromAccountSecret: "",
      toAccountNumber: "",
      amount: ""
    },
    Cheques: []
  });

  const [activeTab, setActiveTab] = useState("general");
  const [profileImg, setProfileImg] = useState(userImage);
  const fileInputRef = useRef(null);
  const [loading, setLoading] = useState(true);
  const [isDepositing, setIsDepositing] = useState(false);
  const [isWithdrawing, setIsWithdrawing] = useState(false);
  const [isTransferring, setIsTransferring] = useState(false);
  const [isCreateChequeModalOpen, setIsCreateChequeModalOpen] = useState(false);
  const [isCancelChequeModalOpen, setIsCancelChequeModalOpen] = useState(false);
  const [isOTPModalOpen, setIsOTPModalOpen] = useState(false);
  const [confirmationData, setConfirmationData] = useState({
    otp: "",
    transactionId: "",
    accountSecret: "",
    amount: "",
    transactionType: ""
  });

  const [chequeForm, setChequeForm] = useState({
    fromAccountName: "",
    toName: "",
    toBankName: "",
    toAccountNumber: "",
    amount: ""
  });
  const [chequeToCancel, setChequeToCancel] = useState("");

  

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const UserID = localStorage.getItem("userId");
        const response = await axios.get(`https://localhost:7152/api/Users/BasicInfo/${UserID}`);
        const { userId, username, email } = response.data;
        
        setUserData(prev => ({
          ...prev,
          general: {
            ...prev.general,
            userId,
            userName: username,
            email
          }
        }));

        // Fetch cheque history
        if (username) {
          const chequeResponse = await axios.get(`https://localhost:7152/api/Cheque/history/${username}`);
          setUserData(prev => ({
            ...prev,
            Cheques: chequeResponse.data
          }));
        }

        setLoading(false);
      } catch (error) {
        console.error("Error fetching user data:", error);
        toast.error("Failed to load user data");
        setLoading(false);
      }
    };

    fetchUserData();
  }, []);

  const handleInputChange = (section, field, value) => {
    setUserData(prev => ({
      ...prev,
      [section]: {
        ...prev[section],
        [field]: value
      }
    }));
  };

  const handleImageUploadClick = () => {
    fileInputRef.current.click();
  };

  const handleImageUpload = (e) => {
    const file = e.target.files?.[0];
    if (file && (file.type === "image/jpeg" || file.type === "image/png")) {
      const reader = new FileReader();
      reader.onload = () => {
        setProfileImg(reader.result);
        toast.success("Profile image updated!");
      };
      reader.readAsDataURL(file);
    } else {
      toast.error("Please upload a JPG or PNG image.");
    }
  };

  const handleCreateAccount = async () => {
    try {
      const response = await axios.post(
        'https://localhost:7152/api/Account/create',
        userData.general.userId, 
        {
          headers: {
            'Content-Type': 'application/json'
          }
        }
      );
      toast.success("Account created successfully!");
    } catch (error) {
      if (error.response || error.response.status === 400 || error.response.status === 500) {
        toast.error("You already have an account");
      } 
    }
  };

  useEffect(() => {
    const fetchBalance = async () => {
      if (userData.general.userId) {
        try {
          const response = await axios.get(
            `https://localhost:7152/api/Account/user/${userData.general.userId}`
          );
          setUserData(prev => ({
            ...prev,
            general: {
              ...prev.general,
              balance: response.data.balance 
            }
          }));
        } catch (error) {
          console.error("Error fetching balance:", error);
          toast.error("Failed to load account balance");
        }
      }
    };
    fetchBalance();
  }, [userData.general.userId]);

  const handleDeposit = async () => {
    if (!userData.Deposit.accountId || !userData.Deposit.amount) {
      toast.error("Please fill all fields");
      return;
    }

    if (parseFloat(userData.Deposit.amount) <= 0) {
      toast.error("Amount must be greater than 0");
      return;
    }

    setIsDepositing(true);
    try {
      const params = new URLSearchParams();
      params.append('accountSecret', userData.Deposit.accountId);
      params.append('amount', parseFloat(userData.Deposit.amount));

      const response = await axios.post(
        `https://localhost:7152/api/Transaction/deposit?${params.toString()}`,
        null,
        {
          headers: {
            'Content-Type': 'application/json',
          }
        }
      );
      
      toast.success("Deposit initiated! Please confirm with OTP.");
      setConfirmationData({
        otp: "",
        transactionId: response.data.transactionId || "",
        accountSecret: userData.Deposit.accountId,
        amount: userData.Deposit.amount,
        transactionType: "deposit"
      });
      setIsOTPModalOpen(true);
      
    } catch (error) {
      console.error("Deposit error:", error);
      toast.error(error.response?.data?.message || "Deposit failed");
    } finally {
      setIsDepositing(false);
    }
  };

  const handleWithdrawal = async () => {
    if (!userData.Withdrawal.accountId || !userData.Withdrawal.amount) {
      toast.error("Please fill all fields");
      return;
    }

    if (parseFloat(userData.Withdrawal.amount) <= 0) {
      toast.error("Amount must be greater than 0");
      return;
    }

    if (parseFloat(userData.Withdrawal.amount) > userData.general.balance) {
      toast.error("Insufficient balance");
      return;
    }

    setIsWithdrawing(true);
    try {
      const params = new URLSearchParams();
      params.append('accountSecret', userData.Withdrawal.accountId);
      params.append('amount', parseFloat(userData.Withdrawal.amount));

      const response = await axios.post(
        `https://localhost:7152/api/Transaction/withdraw?${params.toString()}`,
        null,
        {
          headers: {
            'Content-Type': 'application/json',
          }
        }
      );
      
      toast.success("Withdrawal initiated! Please confirm with OTP.");
      setConfirmationData({
        otp: "",
        transactionId: response.data.transactionId || "",
        accountSecret: userData.Withdrawal.accountId,
        amount: userData.Withdrawal.amount,
        transactionType: "withdrawal"
      });
      setIsOTPModalOpen(true);
      
    } catch (error) {
      console.error("Withdrawal error:", error);
      toast.error(error.response?.data?.message || "Withdrawal failed");
    } finally {
      setIsWithdrawing(false);
    }
  };

  const handleTransfer = async () => {
    if (!userData.Transfer.fromAccountSecret || !userData.Transfer.toAccountNumber || !userData.Transfer.amount) {
      toast.error("Please fill all fields");
      return;
    }

    if (parseFloat(userData.Transfer.amount) <= 0) {
      toast.error("Amount must be greater than 0");
      return;
    }

    if (parseFloat(userData.Transfer.amount) > userData.general.balance) {
      toast.error("Insufficient balance");
      return;
    }

    setIsTransferring(true);
    try {
      const params = new URLSearchParams();
      params.append('fromAccountSecret', userData.Transfer.fromAccountSecret);
      params.append('toAccountNumber', userData.Transfer.toAccountNumber);
      params.append('amount', parseFloat(userData.Transfer.amount));

      const response = await axios.post(
        `https://localhost:7152/api/Transaction/transfer?${params.toString()}`,
        null,
        {
          headers: {
            'Content-Type': 'application/json',
          }
        }
      );
      
      toast.success("Transfer Send to User Account Successfully.");
      
    } catch (error) {
      console.error("Transfer error:", error);
      toast.error(error.response?.data?.message || "Transfer failed, Account Number is not in our bank ");
    } finally {
      setIsTransferring(false);
    }
  };

  const handleOTPChange = (e) => {
    const { name, value } = e.target;
    setConfirmationData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleConfirmTransaction = async () => {
    const { otp, transactionId, accountSecret, amount, transactionType } = confirmationData;

    if (!otp || !transactionId || !accountSecret || (transactionType === "withdrawal" && !amount)) {
      toast.error("Please fill all confirmation fields");
      return;
    }

    try {
      const endpoint = transactionType === "deposit" 
        ? "deposit/confirm" 
        : "withdraw/confirm";

      const params = new URLSearchParams();
      params.append('otp', otp);
      params.append('transactionId', transactionId);
      params.append('accountSecret', accountSecret);
      if (transactionType === "withdrawal") {
        params.append('amount', amount);
      }

      const response = await axios.post(
        `https://localhost:7152/api/Transaction/${endpoint}?${params.toString()}`,
        null,
        {
          headers: {
            'Content-Type': 'application/json',
          }
        }
      );

      const action = transactionType === "deposit" ? "Deposit" : "Withdrawal";
      toast.success(`${action} confirmed successfully!`);

      setIsOTPModalOpen(false);
      setConfirmationData({ 
        otp: "", 
        transactionId: "", 
        accountSecret: "",
        amount: "",
        transactionType: "" 
      });

      // Refresh balance
      const balanceResponse = await axios.get(
        `https://localhost:7152/api/Account/user/${userData.general.userId}`
      );
      setUserData(prev => ({
        ...prev,
        general: {
          ...prev.general,
          balance: balanceResponse.data.balance
        },
        Deposit: transactionType === "deposit" ? {
          accountId: "",
          amount: ""
        } : prev.Deposit,
        Withdrawal: transactionType === "withdrawal" ? {
          accountId: "",
          amount: ""
        } : prev.Withdrawal
      }));

    } catch (error) {
      console.error("Confirmation error:", error);
      toast.error(error.response?.data?.message || "Failed to confirm transaction");
    }
  };


const handleCreateCheque = async () => {
  try {
    const params = new URLSearchParams();
    params.append('fromAccountName', chequeForm.fromAccountName);
    params.append('toName', chequeForm.toName);
    params.append('toBankName', chequeForm.toBankName);
    params.append('toAccountNumber', chequeForm.toAccountNumber);
    params.append('amount', chequeForm.amount);

    const response = await axios.post(
      `https://localhost:7152/api/Cheque/generate?${params.toString()}`,
      null,
      {
        responseType: 'blob' // Important for handling binary data
      }
    );
    
    // Create a blob from the PDF stream
    const file = new Blob([response.data], { type: 'application/pdf' });
    
    // Create a URL for the blob
    const fileURL = URL.createObjectURL(file);
    
    // Create a temporary anchor element to trigger the download
    const tempLink = document.createElement('a');
    tempLink.href = fileURL;
    tempLink.setAttribute('download', `cheque_${Date.now()}.pdf`);
    document.body.appendChild(tempLink);
    
    // Trigger the download
    tempLink.click();
    
    // Clean up
    document.body.removeChild(tempLink);
    URL.revokeObjectURL(fileURL);

    toast.success("Cheque created and downloaded successfully!");
    setIsCreateChequeModalOpen(false);
    setChequeForm({
      fromAccountName: "",
      toName: "",
      toBankName: "",
      toAccountNumber: "",
      amount: ""
    });

    // Refresh cheque list
    const chequeResponse = await axios.get(`https://localhost:7152/api/Cheque/history/${userData.general.userName}`);
    setUserData(prev => ({
      ...prev,
      Cheques: chequeResponse.data
    }));
  } catch (error) {
    console.error("Error creating cheque:", error);
    toast.error(error.response?.data?.message || "Failed to create cheque");
    
    if (error.response?.data?.type === 'application/pdf') {
      const file = new Blob([error.response.data], { type: 'application/pdf' });
      const fileURL = URL.createObjectURL(file);
      window.open(fileURL);
    }
  }
};

  const handleCancelCheque = async () => {
    if (!chequeToCancel) {
      toast.error("Please enter a cheque number");
      return;
    }

    try {
      await axios.post(`https://localhost:7152/api/Cheque/cancel/${chequeToCancel}`);
      
      toast.success("Cheque cancelled successfully!");
      setIsCancelChequeModalOpen(false);
      setChequeToCancel("");

      // Refresh cheque list
      const chequeResponse = await axios.get(`https://localhost:7152/api/Cheque/history/${userData.general.userName}`);
      setUserData(prev => ({
        ...prev,
        Cheques: chequeResponse.data
      }));
    } catch (error) {
      console.error("Error cancelling cheque:", error);
      toast.error(error.response?.data?.message || "Failed to cancel cheque");
    }
  };

  const handleChequeFormChange = (e) => {
    const { name, value } = e.target;
    setChequeForm(prev => ({
      ...prev,
      [name]: value
    }));
  };

























  const renderTabContent = () => {
    if (loading) {
      return <div className="profile-loading">Loading user data...</div>;
    }

    switch (activeTab) {
      case "general":
        return (
          <div className="profile-section">
            <h3 className="profile-section-title">General Information</h3>
            <div className="profile-form-group">
              <label className="profile-form-label">User ID</label>
              <input
                className="profile-form-input"
                value={userData.general.userId}
                disabled
              />
            </div>
            <div className="profile-form-group">
              <label className="profile-form-label">Username</label>
              <input
                className="profile-form-input"
                value={userData.general.userName}
                onChange={(e) => handleInputChange("general", "userName", e.target.value)}
                disabled
              />
            </div>
            <div className="profile-form-group">
              <label className="profile-form-label">Email</label>
              <input
                className="profile-form-input"
                value={userData.general.email}
                onChange={(e) => handleInputChange("general", "email", e.target.value)}
                disabled
              />
            </div>
            <div className="profile-form-group">
              <label className="profile-form-label">Account Balance</label>
              <div className="profile-balance-display">
                {userData.general.balance !== undefined ? (
                  `$${userData.general.balance.toFixed(2)}`
                ) : (
                  "Loading balance..."
                )}
              </div>
            </div>
            <div className="profile-update-notice">
              <div className="profile-update-icon">
                <FiUser />
              </div>
              <div className="profile-update-content">
                <h4>Need to update your personal information?</h4>
                <p>
                  For security reasons, please contact our Customer Service team 
                  from message button in sidebar to update your personal details. We're available 24/7 to assist you.
                </p>
              </div>
            </div>
          </div>
        );

      case "Deposit":
        return (
          <div className="profile-section">
            <h3 className="profile-section-title">Deposit Funds</h3>
            
            <div className="deposit-instructions">
              <div className="deposit-icon">
                <FiDollarSign />
              </div>
              <div className="deposit-content">
                <h4>Make a Deposit</h4>
                <p>
                  Enter your account number and the amount you wish to deposit.
                  You'll receive an OTP via email to confirm the transaction.
                </p>
              </div>
            </div>
            
            <div className="profile-form-group mt-5">
              <label className="profile-form-label">Account Secret Number</label>
              <input
                className="profile-form-input"
                value={userData.Deposit.accountId}
                onChange={(e) => handleInputChange("Deposit", "accountId", e.target.value)}
                placeholder="Enter your account number"
              />
            </div>
            
            <div className="profile-form-group">
              <label className="profile-form-label">Amount ($)</label>
              <input
                className="profile-form-input"
                type="number"
                value={userData.Deposit.amount}
                onChange={(e) => handleInputChange("Deposit", "amount", e.target.value)}
                placeholder="Enter amount"
                min="0.01"
                step="0.01"
              />
            </div>
            
            <div className="profile-form-actions">
              <button
                className={`profile-btn profile-btn-primary ${isDepositing ? 'loading' : ''}`}
                onClick={handleDeposit}
                disabled={isDepositing}
              >
                {isDepositing ? (
                  <>
                    <span className="spinner"></span> Processing...
                  </>
                ) : (
                  <>
                    <FiDollarSign /> Confirm Deposit
                  </>
                )}
              </button>
            </div>
          </div>
        );

      case "Withdrawal":
        return (
          <div className="profile-section">
            <h3 className="profile-section-title">Withdraw Funds</h3>
            
            <div className="profile-form-group mt-5">
              <label className="profile-form-label">Account Secret Number</label>
              <input
                className="profile-form-input"
                value={userData.Withdrawal.accountId}
                onChange={(e) => handleInputChange("Withdrawal", "accountId", e.target.value)}
                placeholder="Enter your account number"
              />
            </div>
            
            <div className="profile-form-group">
              <label className="profile-form-label">Amount ($)</label>
              <input
                className="profile-form-input"
                type="number"
                value={userData.Withdrawal.amount}
                onChange={(e) => handleInputChange("Withdrawal", "amount", e.target.value)}
                placeholder="Enter amount"
                min="0.01"
                step="0.01"
                max={userData.general.balance}
              />
              <div className="balance-info mt-3">
                Available balance: ${userData.general.balance.toFixed(2)}
              </div>
            </div>
            
            <div className="profile-form-actions">
              <button
                className={`profile-btn profile-btn-primary ${isWithdrawing ? 'loading' : ''}`}
                onClick={handleWithdrawal}
                disabled={isWithdrawing}
              >
                {isWithdrawing ? (
                  <>
                    <span className="spinner"></span> Processing...
                  </>
                ) : (
                  <>
                    <FiDollarSign /> Confirm Withdrawal
                  </>
                )}
              </button>
            </div>
          </div>
        );

      case "Transfer":
        return (
          <div className="profile-section">
            <h3 className="profile-section-title">Transfer Money</h3>
            
            <div className="transfer-instructions">
              <div className="transfer-icon">
                <FiSend />
              </div>
              <div className="transfer-content">
                <h4>Send Money to Another Account</h4>
                <p>
                  Transfer funds to other accounts within our bank. 
                  Enter your account details, recipient's account number, and the amount you wish to transfer.
                </p>
              </div>
            </div>
            
            <div className="profile-form-group mt-5">
              <label className="profile-form-label">Your Account Secret</label>
              <input
                className="profile-form-input"
                value={userData.Transfer.fromAccountSecret}
                onChange={(e) => handleInputChange("Transfer", "fromAccountSecret", e.target.value)}
                placeholder="Enter your account secret key"
              />
              <small className="form-hint">This is required to authorize the transfer</small>
            </div>
            
            <div className="profile-form-group">
              <label className="profile-form-label">Recipient Account Number</label>
              <input
                className="profile-form-input"
                value={userData.Transfer.toAccountNumber}
                onChange={(e) => handleInputChange("Transfer", "toAccountNumber", e.target.value)}
                placeholder="Enter recipient's 10-digit account number"
              />
              <small className="form-hint">Make sure the account number is correct</small>
            </div>
            
            <div className="profile-form-group">
              <label className="profile-form-label">Amount ($)</label>
              <input
                className="profile-form-input"
                type="number"
                value={userData.Transfer.amount}
                onChange={(e) => handleInputChange("Transfer", "amount", e.target.value)}
                placeholder="Enter amount to transfer"
                min="0.01"
                step="0.01"
              />
              <div className="balance-info">
                <div className="balance-label">Available Balance:</div>
                <div className="balance-amount">${userData.general.balance.toFixed(2)}</div>
              </div>
            </div>
            
            <div className="profile-form-actions">
              <button
                className={`profile-btn profile-btn-primary ${isTransferring ? 'loading' : ''}`}
                onClick={handleTransfer}
                disabled={isTransferring}
              >
                {isTransferring ? (
                  <>
                    <span className="spinner"></span> Processing Transfer...
                  </>
                ) : (
                  <>
                    <FiSend /> Confirm Transfer
                  </>
                )}
              </button>
            </div>
            
            <div className="transfer-notice">
              <div className="notice-icon">
                <FiCheckCircle />
              </div>
              <div className="notice-content">
                <h5>Transfer Security Notice</h5>
                <p>
                  All transfers are secured with bank-level encryption. 
                  Please double-check recipient details before confirming.
                  Transfers cannot be reversed once completed.
                </p>
              </div>
            </div>
          </div>
        );

case "Cheques":
      return (
        <div className="profile-section">
          <h3 className="profile-section-title">Cheque History</h3>
          
          <div className="cheque-actions mb-4">
            <button 
              className="profile-btn profile-btn-primary"
              onClick={() => setIsCreateChequeModalOpen(true)}
            >
              <FiPlus /> Create New Cheque
            </button>
            <button 
              className="profile-btn profile-btn-danger ms-3"
              onClick={() => setIsCancelChequeModalOpen(true)}
            >
              <FiTrash2 /> Cancel Cheque
            </button>
          </div>
          
          {userData.Cheques.length === 0 ? (
            <div className="no-cheques-message">
              <FiClock className="no-cheques-icon" />
              <p>You don't have any cheque transactions yet.</p>
            </div>
          ) : (
            <div className="cheques-table-container">
              <table className="cheques-table">
                <thead>
                  <tr>
                    <th>Cheque Number</th>
                    <th>Sender</th>
                    <th>Receiver</th>
                    <th>Bank</th>
                    <th>Account Number</th>
                    <th>Amount</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {userData.Cheques.map((cheque, index) => (
                    <tr key={index}>
                      <td>{cheque.chequeNumber}</td>
                      <td>{cheque.senderUserName}</td>
                      <td>{cheque.receiverName}</td>
                      <td>{cheque.receiverBankName}</td>
                      <td>{cheque.receiverAccountNumber}</td>
                      <td className="amount-cell">
                        <FiDollarSign className="dollar-icon" />
                        {cheque.amount.toFixed(2)}
                      </td>
                      <td>
                        <span className={`status-badge ${cheque.status === 0 ? 'pending': cheque.status === 1 ? 'cleared' : 'Cancelled'}`}>
                          {cheque.status === 0 ? (
                            <>
                              <FiClock className="status-icon" /> Pending
                            </>
                          ) : cheque.status === 1 ?  (
                            <>
                              <FiCheck className="status-icon" /> Cleared
                            </>
                          ) : (
                            <>
                              <FiTrash2 className="status-icon" /> Cancelled
                            </>
                          )}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          
          <div className="cheque-notes">
            <div className="note-icon">
              <FiCheckCircle />
            </div>
            <div className="note-content">
              <h4>Cheque Processing Information</h4>
              <p>
                Pending cheques typically clear within 3-5 business days. 
                Cleared cheques will show the settlement date. 
                If you have any questions about a specific cheque, 
                please contact our customer support.
              </p>
            </div>
          </div>
        </div>
      );

      default:
        return null;
    }
  };

  return (
    <div className="profile-wrapper">
      <ToastContainer position="top-right" autoClose={3000} />
      
      <header className="profile-header">
        <div className="profile-header-content">
          <Link to="/profile" className="profile-header-link">
            STC
          </Link>
        </div>
        <button className="profile-action-btn">
          <FiLogOut /> Logout
        </button>
      </header>

      <main className="profile-container">
        <div className="profile-card">
          <div className="profile-sidebar">
            <div className="profile-image-container">
              <img src={profileImg} alt="Profile" className="profile-image" />
              <input
                type="file"
                ref={fileInputRef}
                onChange={handleImageUpload}
                style={{ display: 'none' }}
                accept="image/jpeg, image/png"
              />
          
             
            </div>
            
            <h3 className="profile-name">
              {userData.general.userName} 
            </h3>
            <p className="profile-email">{userData.general.email}</p>
            
            <span className="profile-status profile-status-active">
              Active Member
            </span>
            
            <div className="profile-actions mt-4">
              <button className="profile-action-btn">
                <FiMail /> Message
              </button>

              <button 
                className="profile-action-btn"
                onClick={handleCreateAccount}
              >
                <FiCast /> Create Bank Account
              </button>
            </div>
          </div>

          <div className="profile-content">
            <div className="profile-tabs">
              <div 
                className={`profile-tab ${activeTab === "general" ? "active" : ""}`}
                onClick={() => setActiveTab("general")}
              >
                <FiUser className="mr-2" /> General
              </div>
              <div 
                className={`profile-tab ${activeTab === "Deposit" ? "active" : ""}`}
                onClick={() => setActiveTab("Deposit")}
              >
                <FiBriefcase className="mr-2" /> Deposit
              </div>
              <div 
                className={`profile-tab ${activeTab === "Withdrawal" ? "active" : ""}`}
                onClick={() => setActiveTab("Withdrawal")}
              >
                <FiDollarSign className="mr-2" /> Withdrawal
              </div>
              <div 
                className={`profile-tab ${activeTab === "Transfer" ? "active" : ""}`}
                onClick={() => setActiveTab("Transfer")}
              >
                <FiSend className="mr-2" /> Transfer
              </div>
              <div 
                className={`profile-tab ${activeTab === "Cheques" ? "active" : ""}`}
                onClick={() => setActiveTab("Cheques")}
              >
                <FiDollarSign className="mr-2" /> Cheques
              </div>
            </div>

            {renderTabContent()}
          </div>
        </div>
      </main>
            <Modal
        isOpen={isCreateChequeModalOpen}
        onRequestClose={() => setIsCreateChequeModalOpen(false)}
        className="profile-modal"
        overlayClassName="profile-modal-overlay"
      >
        <div className="profile-modal-content">
          <button 
            className="profile-modal-close"
            onClick={() => setIsCreateChequeModalOpen(false)}
          >
            <FiX />
          </button>
          
          <div className="profile-modal-header">
            <FiPlus className="modal-icon" />
            <h3>Create New Cheque</h3>
            <p>Fill in the details to generate a new cheque</p>
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Your Account Name</label>
            <input
              className="profile-form-input"
              type="text"
              name="fromAccountName"
              value={chequeForm.fromAccountName}
              onChange={handleChequeFormChange}
              placeholder="Enter your account name"
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Recipient Name</label>
            <input
              className="profile-form-input"
              type="text"
              name="toName"
              value={chequeForm.toName}
              onChange={handleChequeFormChange}
              placeholder="Enter recipient's name"
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Recipient Bank Name</label>
            <input
              className="profile-form-input"
              type="text"
              name="toBankName"
              value={chequeForm.toBankName}
              onChange={handleChequeFormChange}
              placeholder="Enter bank name"
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Recipient Account Number</label>
            <input
              className="profile-form-input"
              type="text"
              name="toAccountNumber"
              value={chequeForm.toAccountNumber}
              onChange={handleChequeFormChange}
              placeholder="Enter account number"
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Amount ($)</label>
            <input
              className="profile-form-input"
              type="number"
              name="amount"
              value={chequeForm.amount}
              onChange={handleChequeFormChange}
              placeholder="Enter amount"
              min="0.01"
              step="0.01"
            />
          </div>
          
          <div className="profile-form-actions">
            <button
              className="profile-btn profile-btn-primary"
              onClick={handleCreateCheque}
            >
              <FiCheckCircle /> Generate Cheque
            </button>
          </div>
        </div>
      </Modal>

      {/* Cancel Cheque Modal */}
      <Modal
        isOpen={isCancelChequeModalOpen}
        onRequestClose={() => setIsCancelChequeModalOpen(false)}
        className="profile-modal"
        overlayClassName="profile-modal-overlay"
      >
        <div className="profile-modal-content">
          <button 
            className="profile-modal-close"
            onClick={() => setIsCancelChequeModalOpen(false)}
          >
            <FiX />
          </button>
          
          <div className="profile-modal-header">
            <FiTrash2 className="modal-icon" />
            <h3>Cancel Cheque</h3>
            <p>Enter the cheque number you want to cancel</p>
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Cheque Number</label>
            <input
              className="profile-form-input"
              type="text"
              value={chequeToCancel}
              onChange={(e) => setChequeToCancel(e.target.value)}
              placeholder="Enter cheque number"
            />
          </div>
          
          <div className="profile-form-actions">
            <button
              className="profile-btn profile-btn-danger"
              onClick={handleCancelCheque}
            >
              <FiTrash2 /> Cancel Cheque
            </button>
          </div>
        </div>
      </Modal>



      <Modal
        isOpen={isOTPModalOpen}
        onRequestClose={() => setIsOTPModalOpen(false)}
        className="profile-modal"
        overlayClassName="profile-modal-overlay"
      >
        <div className="profile-modal-content">
          <button 
            className="profile-modal-close"
            onClick={() => setIsOTPModalOpen(false)}
          >
            <FiX />
          </button>
          
          <div className="profile-modal-header">
            <FiCheckCircle className="modal-icon" />
            <h3>Confirm Your {confirmationData.transactionType === "deposit" ? "Deposit" : "Withdrawal"}</h3>
            <p>Please enter the OTP sent to your email</p>
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Transaction Amount</label>
            <input
              className="profile-form-input"
              type="text"
              value={`$${parseFloat(confirmationData.amount || 0).toFixed(2)}`}
              disabled
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">OTP Code</label>
            <input
              className="profile-form-input"
              type="text"
              name="otp"
              value={confirmationData.otp}
              onChange={handleOTPChange}
              placeholder="Enter 6-digit OTP"
              maxLength="6"
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Transaction ID</label>
            <input
              className="profile-form-input"
              type="text"
              name="transactionId"
              value={confirmationData.transactionId}
              onChange={handleOTPChange}
              placeholder="Transaction ID"
            />
          </div>
          
          <div className="profile-form-group">
            <label className="profile-form-label">Account Secret</label>
            <input
              className="profile-form-input"
              type="text"
              name="accountSecret"
              value={confirmationData.accountSecret}
              onChange={handleOTPChange}
              placeholder="Account Secret"
              disabled
            />
          </div>
          
          <div className="profile-form-actions">
            <button
              className="profile-btn profile-btn-primary"
              onClick={handleConfirmTransaction}
            >
              <FiCheckCircle /> Confirm {confirmationData.transactionType === "deposit" ? "Deposit" : "Withdrawal"}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default Profile;