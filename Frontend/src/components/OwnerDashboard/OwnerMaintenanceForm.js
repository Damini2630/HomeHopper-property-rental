import React, { useState, useEffect } from 'react';
import './OwnerMaintenanceForm.css';
import axios from 'axios';
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
 
const OwnerMaintenanceForm = () => {
  const [form, setForm] = useState({
    RequestID: '',
    agentName: '',
    agentContactNo: '',
    status: 'Pending',
    serviceBill: ''
  });
 
  const [tenantRequests, setTenantRequests] = useState([]);
  const [error, setError] = useState(null);
 
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };
 
  const handleSubmit = async (e) => {
    e.preventDefault();
 
    try {
      const token = localStorage.getItem('ownerToken');
      await axios.post('http://localhost:5162/api/maintenance/submit', form, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
 
      await updateStatus(form.RequestID, form.status);
 
      toast.success('Service submitted and status updated successfully');
      setForm({ RequestID: '', agentName: '', agentContactNo: '', status: 'Pending', serviceBill: '' });
      fetchTenantRequests(); // Refresh the list of tenant requests after submission
    } catch (error) {
      console.error('Error submitting service request:', error);
      setError('Error submitting service request');
    }
  };
 
  const updateStatus = async (requestID, status) => {
    try {
      const token = localStorage.getItem('ownerToken');
      await axios.put('http://localhost:5162/api/maintenance/status', { requestID, status }, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
 
      toast.success('Status updated successfully');
    } catch (error) {
      console.error('Error updating status:', error);
      setError('Error updating status');
    }
  };
 
  const fetchTenantRequests = async () => {
    try {
      const response = await axios.get('http://localhost:5162/api/maintenance', {
        headers: { 'Authorization': `Bearer ${localStorage.getItem('ownerToken')}` }
      });
      console.log('Response data:', response.data); // Log the response data
      setTenantRequests(response.data.requests.$values || []);
    } catch (error) {
      console.error('Error fetching tenant requests:', error);
      setError('Error fetching tenant requests');
    }
  };
 
  useEffect(() => {
    fetchTenantRequests();
  }, []);
 
  return (
    <div className="owner-maintenance-form">
      <h2>Maintenance Service Form</h2>
      <form onSubmit={handleSubmit}>
        <label>Request ID</label>
        <input name="RequestID" value={form.RequestID} onChange={handleChange} required />
        <label>Agent Name</label>
        <input name="agentName" value={form.agentName} onChange={handleChange} required />
 
        <label>Agent Contact No</label>
        <input name="agentContactNo" value={form.agentContactNo} onChange={handleChange} required />
 
        <label>Status</label>
        <select name="status" value={form.status} onChange={handleChange}>
          <option>Pending</option>
          <option>In Progress</option>
          <option>Completed</option>
        </select>
 
        <label>Service Bill</label>
        <input name="serviceBill" value={form.serviceBill} onChange={handleChange} required />
 
        <button type="submit">Submit</button>
      </form>
 
      <h2>All Tenant Maintenance Requests</h2>
      {error && <p className="error">{error}</p>}
      <ul>
        {tenantRequests.length > 0 ? (
          tenantRequests.map((request) => (
            <li key={request.requestID}>
              <p>Request ID: {request.requestID}</p>
              <p>Tenant Name: {request.tenantName}</p>
              <p>Tenant Contact No: {request.tenantContactNo}</p>
              <p>Status: {request.status}</p>
              <p>Description: {request.issueDescription}</p>
            </li>
          ))
        ) : (
          <p>No maintenance requests found.</p>
        )}
      </ul>
    </div>
  );
};
 
export default OwnerMaintenanceForm;
 