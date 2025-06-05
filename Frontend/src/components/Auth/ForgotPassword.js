import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './Auth.css';
 
const ForgotPassword = () => {
  const [email, setEmail] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [message, setMessage] = useState('');
  const navigate = useNavigate();
 
  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage('');
 
    try {
      await axios.put('http://localhost:5162/api/Owner/forgot-password', {
        email,
        newPassword
      });
      setMessage('Password reset successfully. Redirecting to login...');
      setTimeout(() => navigate('/sell-login'), 2000);
    } catch (err) {
      setMessage('Error resetting password. Please check your email.');
    }
  };
 
  return (
    <div className="auth-page" style={{ backgroundImage: "url('/assets/auth-bg.jpg')" }}>
      <div className="auth-container">
        <h2>Reset Password</h2>
        {message && <p>{message}</p>}
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              type="email"
              placeholder="Enter your registered email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className="input-group">
            <input
              type="password"
              placeholder="Enter new password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
            />
          </div>
          <button type="submit" className="auth-btn">Reset Password</button>
        </form>
      </div>
    </div>
  );
};
 
export default ForgotPassword;