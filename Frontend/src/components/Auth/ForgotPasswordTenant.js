import React, { useState } from 'react';
import axios from 'axios';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { useNavigate } from 'react-router-dom';
 
function ForgotPasswordTenant() {
  const [formData, setFormData] = useState({ email: '', newPassword: '' });
  const navigate = useNavigate();
 
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };
 
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await axios.put('http://localhost:5162/api/Tenant/reset-password', formData);
      toast.success('Password reset successfully!', {
        position: 'top-center',
        onClose: () => navigate('/rent-login')  // Redirect after toast closes
      });
    } catch (error) {
      toast.error(error.response?.data || 'Failed to reset password', { position: 'top-center' });
    }
  };
 
  return (
    <div className="auth-page">
      <div className="auth-container">
        <h2>Reset Tenant Password</h2>
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              type="email"
              name="email"
              placeholder="Enter your email"
              required
              value={formData.email}
              onChange={handleChange}
            />
          </div>
          <div className="input-group">
            <input
              type="password"
              name="newPassword"
              placeholder="Enter new password"
              required
              value={formData.newPassword}
              onChange={handleChange}
            />
          </div>
          <button type="submit" className="auth-btn">Reset Password</button>
        </form>
        <ToastContainer />
      </div>
    </div>
  );
}
 
export default ForgotPasswordTenant;