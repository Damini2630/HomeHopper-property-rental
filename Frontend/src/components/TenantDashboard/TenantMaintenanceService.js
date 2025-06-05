import React, { useEffect, useState } from 'react';
import axios from 'axios';
import {jwtDecode} from 'jwt-decode'; // Correct import statement
import './RaiseRequestForm.css';
 
const API_BASE = 'http://localhost:5162/api';
 
const TenantMaintenanceService = () => {
  const [tenantId, setTenantId] = useState(null);
  const [token, setToken] = useState(null);
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    propertyID: '',
    ownerID: '',
    issueDescription: '',
    status: 'Pending',
    assignedDate: ''
  });
  const [searchId, setSearchId] = useState('');
  const [searchResult, setSearchResult] = useState(null);
  const [searchError, setSearchError] = useState('');
 
  // Decode token on component mount
  useEffect(() => {
    const storedToken = localStorage.getItem('tenantToken');
    if (storedToken) {
      try {
        const decoded = jwtDecode(storedToken);
        const id = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
        console.log("Stored token: ", storedToken);
        console.log("Decoded token: ", decoded);
        console.log("Tenant id from token: ", id);
        if (id) {
          setTenantId(id);
          setToken(storedToken);
          fetchRequests(id, storedToken);
        }
      } catch (err) {
        console.error("❌ Token decoding failed", err);
      }
    }
  }, []);
 
  // Fetch tenant's requests
  const fetchRequests = async (tenantId, token) => {
    setLoading(true);
    try {
      const response = await axios.get(`${API_BASE}/maintenance/by-tenant/${tenantId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      const requestsData = response.data.$values || [];
      setRequests(Array.isArray(requestsData) ? requestsData : []);
    } catch (error) {
      console.error('❗ Error fetching requests:', error);
      setRequests([]);
    } finally {
      setLoading(false);
    }
  };
 
  // Submit or update request
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!tenantId || !token) {
      alert('Tenant not logged in. Please log in again.');
      return;
    }
    const payload = {
      ...formData,
      tenantID: parseInt(tenantId),
      assignedDate: new Date(formData.assignedDate).toISOString()
    };
    try {
      await axios.post(`${API_BASE}/maintenance`, payload, {
        headers: { Authorization: `Bearer ${token}` }
      });
      alert('✅ Request submitted successfully!');
      resetForm();
      fetchRequests(tenantId, token);
    } catch (error) {
      alert('❌ Submission failed');
      console.error('❗ Error submitting request:', error);
    }
  };
 
  // Search maintenance request by ID
  const handleSearch = async () => {
    setSearchResult(null);
    setSearchError('');
    if (!searchId.trim()) return;
    try {
      const response = await axios.get(`${API_BASE}/maintenance/${searchId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setSearchResult(response.data?.request || null);
    } catch (err) {
      console.error(err);
      setSearchError('❌ No request found or unauthorized access.');
    }
  };
 
  const resetForm = () => {
    setFormData({
      propertyID: '',
      ownerID: '',
      issueDescription: '',
      status: 'Pending',
      assignedDate: ''
    });
  };
 
  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };
 
  return (
    <div className="maintenance-layout">
      {/* Left Side: Previous Requests */}
      <div className="maintenance-left">
        <h3>My Maintenance Requests</h3>
        {loading ? (
          <p>Loading...</p>
        ) : requests.length === 0 ? (
          <p>No requests found.</p>
        ) : (
          <ul className="request-list">
            {requests.map(req => (
              <li key={req.requestID}>
                <strong>{req.issueDescription}</strong> – <em>{req.status}</em><br />
                Request ID: {req.requestID}<br />
                Date: {new Date(req.assignedDate).toLocaleDateString()}<br />
                Property ID: {req.propertyID}<br />
                Owner ID: {req.ownerID}<br />
              </li>
            ))}
          </ul>
        )}
        {/* Search Section */}
        <div style={{ marginTop: '20px' }}>
          <h4>🔍 Search Request by ID</h4>
          <input
            type="number"
            placeholder="Enter Request ID"
            value={searchId}
            onChange={(e) => setSearchId(e.target.value)}
            style={{ width: '100%', padding: '8px' }}
          />
          <button onClick={handleSearch} style={{ marginTop: '8px', padding: '8px 12px' }}>Search</button>
          {searchError && <p style={{ color: 'red' }}>{searchError}</p>}
          {searchResult && (
            <div style={{ marginTop: '15px', background: '#f0f0f0', padding: '12px', borderRadius: '5px' }}>
              <p><strong>Description:</strong> {searchResult.issueDescription}</p>
              <p><strong>Status:</strong> {searchResult.status}</p>
              <p><strong>Date:</strong> {new Date(searchResult.assignedDate).toLocaleDateString()}</p>
              <p><strong>Property ID:</strong> {searchResult.propertyID}</p>
              <p><strong>Owner ID:</strong> {searchResult.ownerID}</p>
              <p><strong>Request ID:</strong> {searchResult.requestID}</p>
            </div>
          )}
        </div>
      </div>
      {/* Right Side: Form */}
      <div className="maintenance-right">
        <h3>New Maintenance Request</h3>
        <form onSubmit={handleSubmit}>
          <input type="number" name="propertyID" placeholder="Property ID" value={formData.propertyID} onChange={handleChange} required />
          <input type="number" name="ownerID" placeholder="Owner ID" value={formData.ownerID} onChange={handleChange} required />
          <textarea name="issueDescription" placeholder="Describe the issue" value={formData.issueDescription} onChange={handleChange} required />
          <input type="date" name="assignedDate" value={formData.assignedDate} onChange={handleChange} required />
          <button type="submit">Submit</button>
        </form>
      </div>
    </div>
  );
};
 
export default TenantMaintenanceService;