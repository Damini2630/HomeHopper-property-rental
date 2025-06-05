import React, { useState, useEffect, createContext, useContext } from 'react';
import { Link, Outlet, useNavigate, OutletContext } from 'react-router-dom';
import { FaUser, FaSignOutAlt, FaWrench, FaSearch, FaFileAlt, FaMoneyBillAlt } from 'react-icons/fa';
import { jwtDecode } from 'jwt-decode';
import './TenantDashboard.css';
import axios from 'axios';
 
// Create a context to hold the tenant's applied property IDs
const AppliedPropertiesContext = createContext([]);
export const useAppliedProperties = () => useContext(AppliedPropertiesContext);
 
function TenantLayout() {
    const navigate = useNavigate();
    const [tenantId, setTenantId] = useState(null);
    const [tenantName, setTenantName] = useState('Tenant');
    const [loadingProfile, setLoadingProfile] = useState(true);
    const [errorProfile, setErrorProfile] = useState('');
    const [appliedPropertyIds, setAppliedPropertyIds] = useState([]);
    const [loadingApplications, setLoadingApplications] = useState(true);
    const [errorApplications, setErrorApplications] = useState(null);
 
    useEffect(() => {
        const token = localStorage.getItem('tenantToken');
        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                const currentTenantId = decodedToken.TenantID || decodedToken.sub || decodedToken.userId || decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
                setTenantId(currentTenantId);
 
                const fetchTenantProfile = async () => {
                    setLoadingProfile(true);
                    try {
                        const response = await axios.get('http://localhost:5162/api/tenant/profile', {
                            headers: {
                                   Authorization: `Bearer ${token}`, // Assuming Bearer token authentication
                                  },
                                  });
                        setTenantName(response.data.name);
                        setLoadingProfile(false);
                    } catch (error) {
                        console.error('Error fetching tenant profile:', error);
                        setErrorProfile('Failed to load tenant information.');
                        setLoadingProfile(false);
                    }
                };
 
                const fetchTenantApplications = async () => {
                    setLoadingApplications(true);
                    try {
                        const response = await axios.get(`http://localhost:5162/api/RentalApplication/tenant`, {
                            headers: {
                                'Authorization': `Bearer ${token}`,
                            },
                        });
                        const responseData = response.data;
                        const appliedIds = [];
                        if (responseData?.$values) {
                            responseData.$values.forEach(app => {
                                if (app?.propertyID) {
                                    appliedIds.push(app.propertyID);
                                } else if (app?.property?.propertyID) {
                                    appliedIds.push(app.property.propertyID);
                                }
                            });
                        }
                        setAppliedPropertyIds(appliedIds);
                        setLoadingApplications(false);
                    } catch (error) {
                        console.error('Error fetching tenant applications:', error);
                        setErrorApplications('Failed to load your applications.');
                        setLoadingApplications(false);
                    }
                };
 
                fetchTenantProfile();
                fetchTenantApplications();
 
            } catch (error) {
                console.error('Error decoding token:', error);
                localStorage.removeItem('tenantToken');
                navigate('/rent-login');
            }
        } else {
            navigate('/rent-login');
        }
    }, [navigate]);
 
    const handleLogout = () => {
        localStorage.removeItem('tenantToken');
        navigate('/rent-login');
    };
 
    return (
        <div className="tenant-dashboard">
            <div className="sidebar">
                <div className="profile-summary">
                    <div className="user-icon">
                        <FaUser size={48} />
                    </div>
                    <h3>{loadingProfile ? 'Loading...' : errorProfile ? 'Error' : tenantName}</h3>
                    <p>Tenant Dashboard</p>
                    {tenantId && <p className="tenant-id">ID: {tenantId}</p>}
                </div>
                <nav>
                    <Link to="/tenant/search" className="nav-link">
                        <FaSearch className="icon" /> Find Properties
                    </Link>
                    <Link to="/tenant/applications" className="nav-link">
                        <FaFileAlt className="icon" /> My Applications
                    </Link>
                    <Link to="/tenant/leases" className="nav-link">
                        <FaFileAlt className="icon" /> My Leases
                    </Link>
                    <Link to="/tenant/payments" className="nav-link">
                        <FaMoneyBillAlt className="icon" /> Payments
                    </Link>
                    <Link to="/tenant/profile" className="nav-link">
                        <FaUser className="icon" /> Profile
                    </Link>
                    <Link to="/tenant/maintenance" className="nav-link">
                        <FaWrench className="icon" /> Maintenance Service
                    </Link>
                    <button onClick={handleLogout} className="nav-link logout-btn">
                        <FaSignOutAlt className="icon" /> Logout
                    </button>
                </nav>
            </div>
            <div className="main-content">
                <AppliedPropertiesContext.Provider value={appliedPropertyIds}>
                    <Outlet />
                </AppliedPropertiesContext.Provider>
            </div>
        </div>
    );
}
 
export default TenantLayout;