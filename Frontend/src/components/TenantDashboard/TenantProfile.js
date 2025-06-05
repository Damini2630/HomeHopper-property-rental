import { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { FaUser, FaEnvelope, FaPhone, FaHome, FaLock, FaSave, FaEdit, FaTrash } from 'react-icons/fa';
import './TenantDashboard.css';
 
function TenantProfile() {
    const [profile, setProfile] = useState({
        name: '',
        email: '',
        phone: '',
        currentAddress: '',
        tenantID: '',
        password: ''
    });
 
    const [editMode, setEditMode] = useState(false);
    const [error, setError] = useState(null);
    const navigate = useNavigate();
 
    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const token = localStorage.getItem('tenantToken');
                if (!token) throw new Error('JWT token not found');
 
                const response = await axios.get('http://localhost:5162/api/Tenant/profile', {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
 
                if (!response.data) throw new Error('Profile data not found');
                setProfile(response.data);
            } catch (error) {
                console.error('Error fetching profile:', error);
                setError(error.message);
            }
        };
 
        fetchProfile();
    }, []);
 
    const handleEditToggle = () => setEditMode(true);
 
    const handleChange = (e) => {
        const { name, value } = e.target;
        setProfile((prev) => ({ ...prev, [name]: value }));
    };
 
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const token = localStorage.getItem('tenantToken');
            if (!token) throw new Error('JWT token not found');
 
            const updatedProfile = {
                name: profile.name,
                email: profile.email,
                contactDetails: profile.contactDetails,
                
                password: profile.password || profile.oldPassword, // retain old if blank
            };
 
            const response = await axios.put('http://localhost:5162/api/Tenant/update-profile', updatedProfile, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });
 
            if (response.status === 200) {
                alert('Profile updated successfully!');
                setEditMode(false);
            } else {
                alert('Failed to update profile.');
            }
        } catch (error) {
            console.error('Edit error:', error);
            alert('Failed to update profile.');
        }
    };
 
    const handleDelete = async () => {
        try {
            const token = localStorage.getItem('tenantToken');
            await axios.delete('http://localhost:5162/api/Tenant/delete', {
                headers: { Authorization: `Bearer ${token}` },
            });
            alert('Profile deleted successfully!');
            localStorage.removeItem('tenantToken');
            navigate('/rent-login');
        } catch (error) {
            console.error('Delete error:', error);
            alert('Failed to delete profile.');
        }
    };
 
    return (
        <div className="profile-page">
            <h2>My Profile</h2>
            {error && <p className="error-message">{error}</p>}
 
            <div className="profile-card">
                <div className="profile-header">
                    <div className="avatar">
                        <FaUser size={40} />
                    </div>
                    <h3>{profile.name}</h3>
                    <p>Tenant</p>
                </div>
 
                {!editMode ? (
                    <div className="profile-details">
                        <p><FaEnvelope /> {profile.email}</p>
                        <p><FaPhone /> {profile.contactDetails}</p>
                        
                        <p><FaLock /> ******</p>
                    </div>
                ) : (
                    <form className="edit-profile-form" onSubmit={handleSubmit}>
                        <label>Name:</label>
                        <input name="name" value={profile.name} onChange={handleChange} />
 
                        <label>Email:</label>
                        <input name="email" value={profile.email} onChange={handleChange} />
 
                        <label>Phone:</label>
                        <input name="phone" value={profile.contactDetails} onChange={handleChange} />
 
                        
 
                        <label>New Password:</label>
                        <input
                            type="password"
                            name="password"
                            placeholder="Leave blank to keep old password"
                            value={profile.password || ''}
                            onChange={handleChange}
                        />
 
                        <button type="submit"><FaSave /> Save Changes</button>
                    </form>
                )}
 
                {!editMode && (
                    <div className="profile-actions">
                        <button className="btn-edit" onClick={handleEditToggle}>
                            <FaEdit /> Edit Profile
                        </button>
                        <button className="btn-delete" onClick={handleDelete}>
                            <FaTrash /> Delete Profile
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}
 
export default TenantProfile;