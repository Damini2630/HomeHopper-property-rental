import React from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './OwnerDashboard.css';
 
function DeleteProfile() {
  const navigate = useNavigate();
 
  const handleDelete = async () => {
    toast.info(
      ({ closeToast }) => (
        <div>
          <p>Are you sure you want to delete your profile?</p>
          <div style={{ display: 'flex', justifyContent: 'center', gap: '10px' }}>
            <button
              onClick={async () => {
                closeToast();
                try {
                  const token = localStorage.getItem('ownerToken');
                  await axios.delete('http://localhost:5162/api/Owner/delete', {
                    headers: { Authorization: `Bearer ${token}` }
                  });
 
                  toast.success('Profile deleted successfully!', {
                    position: 'top-center',
                    onClose: () => {
                      localStorage.removeItem('ownerToken');
                      navigate('/sell-register');
                    }
                  });
                } catch (error) {
                  toast.error('Failed to delete profile. Please try again.', {
                    position: 'top-center'
                  });
                  console.error('Delete error:', error);
                }
              }}
              className="btn btn-danger"
            >
              Yes
            </button>
            <button onClick={closeToast} className="btn btn-secondary">No</button>
          </div>
        </div>
      ),
      { position: 'top-center', autoClose: false }
    );
  };
 
  return (
    <div className="form-page">
      <h2>Delete Profile</h2>
      <p>Once deleted, your profile and properties will be permanently removed.</p>
      <button onClick={handleDelete} className="btn-danger">
        Delete My Profile
      </button>
      <ToastContainer />
    </div>
  );
}
 
export default DeleteProfile;