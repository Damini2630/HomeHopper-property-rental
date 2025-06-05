import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { FaSpinner, FaExclamationTriangle, FaTrash } from 'react-icons/fa';
import { ToastContainer, toast } from 'react-toastify';
import { confirmAlert } from 'react-confirm-alert';
import 'react-toastify/dist/ReactToastify.css';
import 'react-confirm-alert/src/react-confirm-alert.css';
import './OwnerDashboard.css';
 
function DeleteProperty() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [property, setProperty] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isDeleting, setIsDeleting] = useState(false);
 
  useEffect(() => {
    const fetchPropertyDetails = async () => {
      try {
        const token = localStorage.getItem('ownerToken');
        const response = await axios.get(`http://localhost:5162/api/property/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setProperty(response.data);
        setLoading(false);
      } catch (err) {
        console.error('Error fetching property details:', err);
        setError(err.response?.data?.message || 'Failed to load property details.');
        setLoading(false);
      }
    };
 
    fetchPropertyDetails();
  }, [id]);
 
  const handleDelete = () => {
    confirmAlert({
      title: 'Confirm Deletion',
      message: `Are you sure you want to permanently delete "${property?.propertyName}"?`,
      buttons: [
        {
          label: 'Yes',
          onClick: proceedToDelete,
        },
        {
          label: 'No',
          onClick: () => toast.info('Deletion cancelled.', { position: 'top-center' }),
        },
      ],
    });
  };
 
  const proceedToDelete = async () => {
    setIsDeleting(true);
    try {
      const token = localStorage.getItem('ownerToken');
      await axios.delete(`http://localhost:5162/api/property/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      toast.success('Property deleted successfully.', { position: 'top-center' });
      navigate('/owner/properties');
    } catch (err) {
      console.error('Error deleting property:', err);
      toast.error(err.response?.data?.message || 'Failed to delete property.', {
        position: 'top-center',
      });
    } finally {
      setIsDeleting(false);
    }
  };
 
  if (loading) {
    return (
      <div className="loading-container">
        <FaSpinner className="spinner-icon" />
        <p>Loading property details for deletion...</p>
      </div>
    );
  }
 
  if (error) {
    return (
      <div className="error-container">
        <FaExclamationTriangle className="error-icon" />
        <h3>Error</h3>
        <p>{error}</p>
        <Link to="/owner/properties" className="btn-secondary">
          Back to My Properties
        </Link>
      </div>
    );
  }
 
  if (!property) {
    return <div>Property not found.</div>;
  }
 
  return (
    <div className="delete-property-container">
      <ToastContainer />
      <h2><FaTrash /> Confirm Delete Property</h2>
      <p>Are you sure you want to delete the following property?</p>
      <h3>{property.propertyName}</h3>
      <p>
        {property.address}, {property.state}, {property.country}
      </p>
      <div className="delete-actions">
        <button onClick={handleDelete} className="btn-danger" disabled={isDeleting}>
          {isDeleting ? <FaSpinner className="spinner" spin /> : 'Delete'}
        </button>
        <Link to="/owner/properties" className="btn-secondary">
          Cancel
        </Link>
      </div>
    </div>
  );
}
 
export default DeleteProperty;