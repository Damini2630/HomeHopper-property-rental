import { useEffect, useState } from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import {
  FaHome,
  FaPlus,
  FaClipboardList,
  FaFileContract,
  FaBell,
  FaUser,
  FaSignOutAlt
} from 'react-icons/fa';
import './OwnerDashboard.css';
import axios from 'axios';
 
function OwnerLayout() {
  const navigate = useNavigate();
  const [ownerName, setOwnerName] = useState('Owner');
 
  useEffect(() => {
    const fetchOwnerProfile = async () => {
      const token = localStorage.getItem('ownerToken');
      if (!token) return;
 
      try {
        const response = await axios.get('http://localhost:5162/api/Owner', {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
 
        const { name } = response.data;
        if (name) {
          setOwnerName(name);
        }
      } catch (error) {
        console.error('❌ Failed to fetch owner profile:', error);
        // fallback: try extracting from token
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          const fallbackName =
            payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
            payload['name'] || 'Owner';
          setOwnerName(fallbackName);
        } catch {
          setOwnerName('Owner');
        }
      }
    };
 
    fetchOwnerProfile();
  }, []);
 
  const handleLogout = () => {
    localStorage.removeItem('ownerToken');
    navigate('/sell-login');
  };
 
  return (
    <div className="owner-dashboard">
      {/* Sidebar */}
      <div className="sidebar">
        <div className="profile-summary">
          <div className="user-icon">
            <FaUser size={48} />
          </div>
          <h3>{ownerName}</h3>
          <p>Premium Member</p>
        </div>
 
        <nav>
          
          <Link to="/owner/profile" className="nav-link">
            <FaUser className="icon" /> Profile
          </Link>
          <Link to="/owner/add-property" className="nav-link">
            <FaPlus className="icon" /> Add Property
          </Link>
          <Link to="/owner/properties" className="nav-link">
            <FaHome className="icon" /> My Properties
          </Link>
          <Link to="/owner/applications" className="nav-link">
            <FaClipboardList className="icon" /> Applications
          </Link>
          <Link to="/owner/leases" className="nav-link">
            <FaFileContract className="icon" /> Leases
          </Link>
          <Link to="/owner/notifications" className="nav-link">
            <FaBell className="icon" /> Notifications
          </Link>
          <Link to="/owner/maintenance-service" className="nav-link">          
              <FaClipboardList className="icon" /> Maintenance Service
             </Link>
          <button onClick={handleLogout} className="nav-link logout-btn">
            <FaSignOutAlt className="icon" /> Logout
          </button>
        </nav>
      </div>
 
      {/* Main Content */}
      <div className="main-content">
        <Outlet />
      </div>
    </div>
  );
}
 
export default OwnerLayout;
 