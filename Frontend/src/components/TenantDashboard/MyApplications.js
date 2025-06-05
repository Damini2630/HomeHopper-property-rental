import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import './RentalApplicationForm.css';
import { FaSpinner, FaExclamationCircle } from 'react-icons/fa';
import { confirmAlert } from 'react-confirm-alert';
import 'react-confirm-alert/src/react-confirm-alert.css';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
 
function MyApplications() {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [leaseAgreementsSubmittedForProperty, setLeaseAgreementsSubmittedForProperty] = useState({});
  const navigate = useNavigate();
  const tenantId = localStorage.getItem('tenantId');
 
  useEffect(() => {
    const fetchApplications = async () => {
      const token = localStorage.getItem('tenantToken');
      if (!token) {
        setError('Unauthorized. Please log in again.');
        setLoading(false);
        return;
      }
      try {
        const response = await axios.get('http://localhost:5162/api/RentalApplication/tenant', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        const responseData = response.data;
        const allApplications = [];
 
        if (responseData?.$values && responseData.$values.length > 0) {
          for (const item of responseData.$values) {
            if (item?.rentalApplicationID) {
              allApplications.push(item);
              if (['accepted', 'approved'].includes(item.status?.toLowerCase()) && item.propertyID) {
                await checkIfLeaseExists(item.propertyID);
              }
            }
            if (item?.property?.rentalApplications?.$values) {
              const propertyName = item.property.propertyName;
              for (const nestedApp of item.property.rentalApplications.$values) {
                if (nestedApp?.rentalApplicationID) {
                  allApplications.push({ ...nestedApp, propertyName: propertyName, propertyID: item.propertyID, propertyImageUrl: item.property?.propertyImageUrl });
                  if (['accepted', 'approved'].includes(nestedApp.status?.toLowerCase()) && item.propertyID) {
                    await checkIfLeaseExists(item.propertyID);
                  }
                }
              }
            } else if (item?.property) {
              allApplications.push({ ...item, propertyImageUrl: item.property.propertyImageUrl, propertyID: item.propertyID });
            }
          }
        }
 
        setApplications(allApplications);
      } catch (err) {
        console.error('Error fetching applications:', err);
        setError('Failed to load applications.');
      } finally {
        setLoading(false);
      }
    };
 
    fetchApplications();
  }, []);
 
  const checkIfLeaseExists = async (propertyId) => {
    try {
      const response = await axios.get(`http://localhost:5162/api/LeaseAgreement/property/${propertyId}/tenant/lease-exists`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('tenantToken')}`
        }
      });
      setLeaseAgreementsSubmittedForProperty(prevState => ({
        ...prevState,
        [propertyId]: response.data.exists,
      }));
    } catch (error) {
      console.error('Error checking lease agreement:', error);
    }
  };
 
  const getStatusClass = (status) => {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'status-pending';
      case 'approved':
      case 'accepted':
        return 'status-approved';
      case 'rejected':
      case 'declined':
        return 'status-rejected';
      default:
        return 'status-unknown';
    }
  };
 
  const handleDelete = async (applicationId) => {
    confirmAlert({
      title: 'Confirm Delete',
      message: 'Are you sure you want to delete this application?',
      buttons: [
        {
          label: 'Yes',
          onClick: async () => {
            try {
              const token = localStorage.getItem('tenantToken');
              await axios.delete(`http://localhost:5162/api/RentalApplication/${applicationId}`, {
                headers: {
                  'Authorization': `Bearer ${token}`,
                },
              });
              setApplications(applications.filter(app => app.rentalApplicationID !== applicationId));
              toast.success('Application deleted successfully!');
            } catch (error) {
              console.error('Error deleting application:', error);
              toast.error('Failed to delete application.');
            }
          }
        },
        {
          label: 'No'
        }
      ]
    });
  };
 
  return (
    <div className="tenant-applications">
      <h2>My Rental Applications</h2>
      <ToastContainer position="top-center" autoClose={3000} />
      {loading ? (
        <div className="loading">
          <FaSpinner className="spinner" /> Loading applications...
        </div>
      ) : error ? (
        <div className="error-message">
          <FaExclamationCircle /> {error}
        </div>
      ) : applications.length === 0 ? (
        <p>No applications submitted yet.</p>
      ) : (
        applications.map(app => (
          <div key={app.rentalApplicationID} className="application-card">
            {app.propertyImageUrl && (
              <div className="property-image-container">
                <img src={app.propertyImageUrl} alt={app.propertyName || app.property?.propertyName} className="property-image" />
              </div>
            )}
            <div className="application-details">
              <h3>Application for {app.propertyName || app.property?.propertyName}</h3>
              <p><strong>Status:</strong> <span className={`application-status ${getStatusClass(app.status)}`}>{app.status}</span></p>
              <p><strong>Number of People:</strong> {app.noOfPeople}</p>
              <p><strong>Preferred Stay Period:</strong> {app.stayPeriod}</p>
              <p><strong>Tentative Start Date:</strong> {app.tentativeStartDate?.split('T')[0]}</p>
              <p><strong>Permanent Address:</strong> {app.permanentAddress}</p>
              <p><strong>State:</strong> {app.state}</p>
              <p><strong>Country:</strong> {app.country}</p>
              <p><strong>Specific Requirements:</strong> {app.specificRequirements || 'N/A'}</p>
            </div>
 
            <div className="application-actions">
              {app.status?.toLowerCase() === 'pending' && (
                <Link to={`/tenant/applications/edit/${app.rentalApplicationID}`} className="edit-btn">
                  Edit Application
                </Link>
              )}
              {app.status?.toLowerCase() !== 'approved' && (
                <button onClick={() => handleDelete(app.rentalApplicationID)} className="delete-btn">
                  Delete Application
                </button>
              )}
            </div>
 
            {['accepted', 'approved'].includes(app.status?.toLowerCase()) &&
              !leaseAgreementsSubmittedForProperty[app.propertyID] && (
                <div className="lease-button-container">
                  {console.log("MyApplications - tenantId right before Link:", tenantId)}
                  <Link
                    to={`/tenant/lease-agreement/${app.rentalApplicationID}?propertyId=${app.propertyID}&tenantId=${tenantId}`}
                    className="lease-btn"
                  >
                    Proceed to Lease Agreement
                  </Link>
                </div>
              )}
 
            {['accepted', 'approved'].includes(app.status?.toLowerCase()) &&
              leaseAgreementsSubmittedForProperty[app.propertyID] && (
                <div className="lease-submitted-message">
                  Lease Agreement Submitted for this Property
                </div>
              )}
          </div>
        ))
      )}
    </div>
  );
}
 
export default MyApplications;