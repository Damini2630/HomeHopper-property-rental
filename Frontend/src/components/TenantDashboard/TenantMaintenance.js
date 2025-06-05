import React, { useEffect, useState } from 'react';
import axios from 'axios';
 
const TenantMaintenanceService = () => {
  const [requests, setRequests] = useState([]);
  const [formData, setFormData] = useState({
    propertyID: '',
    issueDescription: '',
    status: 'Pending',
    assignedDate: '',
  });
 
  const tenantId = localStorage.getItem('tenantId');
  const token = localStorage.getItem('tenantToken');
 
  const fetchRequests = async () => {
    try {
      if (!tenantId || !token) {
        console.error("❗ Tenant ID or token not found in local storage.");
        return;
      }
 
      const response = await axios.get(`http://localhost:5162/api/maintenance/by-tenant/${tenantId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
 
      console.log("✅ Requests fetched:", response.data);
      setRequests(response.data);
    } catch (error) {
      console.error("❗ Error fetching requests:", error);
    }
  };
 
  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };
 
  const handleSubmit = async (e) => {
    e.preventDefault();
 
    try {
      const payload = new FormData();
      payload.append('PropertyID', formData.propertyID);
      payload.append('TenantID', tenantId);
      payload.append('OwnerID', '1'); // you can update this dynamically if needed
      payload.append('IssueDescription', formData.issueDescription);
      payload.append('Status', formData.status);
      payload.append('AssignedDate', formData.assignedDate);
 
      const response = await axios.post(`http://localhost:5162/api/maintenance`, payload, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
   
 
      console.log("✅ Request submitted:", response.data);
      fetchRequests(); // Refresh list
    } catch (error) {
      console.error("❗ Error submitting request:", error);
    }
  };
 
  useEffect(() => {
    fetchRequests();
  }, []);
 
  return (
<div className="tenant-maintenance-container">
<h2>Maintenance Request Form</h2>
<form onSubmit={handleSubmit}>
<label>
          Property ID:
<input type="text" name="propertyID" value={formData.propertyID} onChange={handleChange} required />
</label>
<label>
          Issue Description:
<textarea name="issueDescription" value={formData.issueDescription} onChange={handleChange} required />
</label>
<label>
          Assigned Date:
<input type="date" name="assignedDate" value={formData.assignedDate} onChange={handleChange} required />
</label>
<button type="submit">Submit Request</button>
</form>
 
      <h3>Your Maintenance Requests</h3>
<ul>
        {requests.map((r) => (
<li key={r.requestID}>
            #{r.requestID} - {r.issueDescription} ({r.status})
</li>
        ))}
</ul>
</div>
  );
};
 
export default TenantMaintenanceService;