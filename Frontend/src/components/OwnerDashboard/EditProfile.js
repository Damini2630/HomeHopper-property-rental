import React, { useState, useEffect } from 'react'; // Import useEffect
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import { FaUser, FaEnvelope, FaPhone, FaLock, FaSave } from 'react-icons/fa';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './OwnerDashboard.css';

function EditProfile() {
  const location = useLocation();
  const navigate = useNavigate();

  // Initialize profile state with empty strings as defaults
  const [profile, setProfile] = useState({
    name: '',
    email: '',
    contactDetails: '',
    password: '',
  });

  // Use useEffect to set the profile data when the component mounts
  useEffect(() => {
    if (location.state?.profile) {
      setProfile({
        name: location.state.profile.name || '',
        email: location.state.profile.email || '',
        contactDetails: location.state.profile.phone || '',
        password: '', // We don't want to pre-fill the password for security
      });
    } else {
      // Handle the case where location.state.profile is not available
      // You might want to fetch the profile data from the server here
      // or navigate the user back to the profile page.
      console.warn('Profile data not received via navigation state.');
      // Example: Fetch profile from server
      const fetchProfile = async () => {
        try {
          const token = localStorage.getItem('ownerToken');
          if (!token) throw new Error('JWT token not found');
          const decodedToken = jwtDecode(token);
          const ownerId = decodedToken.ownerId;
          if (!ownerId) throw new Error('Owner ID not found in token');

          const response = await axios.get(`http://localhost:5162/api/Owner/${ownerId}`, {
            headers: { Authorization: `Bearer ${token}` },
          });

          if (response.status === 200) {
            const fetchedProfile = response.data;
            setProfile({
              name: fetchedProfile.name || '',
              email: fetchedProfile.email || '',
              contactDetails: fetchedProfile.contactDetails || '',
              password: '',
            });
          } else {
            toast.error('Failed to load profile data.', { position: 'top-center' });
            navigate('/owner/profile'); // Redirect if fetch fails
          }
        } catch (error) {
          console.error('Error fetching profile:', error);
          toast.error('Failed to load profile data: ' + error.message, { position: 'top-center' });
          navigate('/owner/profile'); // Redirect on error
        }
      };

      fetchProfile();
    }
  }, [location.state, navigate]); // Re-run effect if location.state changes

  const handleChange = (e) => {
    const { name, value } = e.target;
    setProfile((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('ownerToken');
      if (!token) throw new Error('JWT token not found');

      const decodedToken = jwtDecode(token);
      const ownerId = decodedToken.ownerId;
      if (!ownerId) throw new Error('Owner ID not found in token');

      const payload = {
        ownerId,
        name: profile.name,
        email: profile.email,
        contactDetails: profile.contactDetails,
        password: profile.password || (location.state?.profile?.password || ''), // Fallback to potentially old password
      };

      const response = await axios.put('http://localhost:5162/api/Owner/update', payload, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (response.status === 200) {
        toast.success('Profile updated successfully!', { position: 'top-center' });
        setTimeout(() => navigate('/owner/profile'), 1500);
      } else {
        throw new Error('Failed to update profile');
      }
    } catch (error) {
      console.error(error);
      toast.error('Update failed: ' + error.message, { position: 'top-center' });
    }
  };

  return (
    <div className="edit-profile-page">
      <h2>Edit Profile</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label><FaUser /> Full Name</label>
          <input
            type="text"
            name="name"
            value={profile.name}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label><FaEnvelope /> Email</label>
          <input
            type="text"
            name="email"
            value={profile.email}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label><FaPhone /> Phone</label>
          <input
            type="tel"
            name="contactDetails"
            value={profile.contactDetails}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label><FaLock /> New Password</label>
          <input
            type="password"
            name="password"
            value={profile.password}
            onChange={handleChange}
            placeholder="Enter new password (optional)"
          />
        </div>

        <button type="submit" className="btn-save">
          <FaSave /> Save Changes
        </button>
      </form>

      <ToastContainer />
    </div>
  );
}

export default EditProfile;