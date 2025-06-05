import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { FaFileContract, FaDownload, FaCalendarAlt, FaCheckCircle, FaTimesCircle, FaClock, FaMoneyBill } from 'react-icons/fa';
import './TenantDashboard.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function MyLeases() {
  const [leases, setLeases] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const [fullLeaseData, setFullLeaseData] = useState([]);
  const [resolvedLeases, setResolvedLeases] = useState({});
  const [deletingLeaseId, setDeletingLeaseId] = useState(null);

  useEffect(() => {
    const fetchMyLeases = async () => {
      setLoading(true);
      setError('');
      const tenantToken = localStorage.getItem('tenantToken');

      if (!tenantToken) {
        setError('Authentication token not found.');
        setLoading(false);
        return;
      }

      try {
        const response = await axios.get(
          'http://localhost:5162/api/LeaseAgreement/tenant/leases',
          {
            headers: {
              Authorization: `Bearer ${tenantToken}`,
            },
          }
        );
        console.log('Raw Response Data:', response.data);
        setFullLeaseData(response.data?.$values || []);
        setLeases(response.data?.$values || []);
        setLoading(false);
      } catch (err) {
        console.error('Error fetching leases:', err);
        setError('Failed to fetch lease agreements.');
        setLoading(false);
      }
    };

    fetchMyLeases();
  }, []);

  const findReferencedObject = (data, refId) => {
    if (data?.$id === refId) {
      return data;
    }
    if (Array.isArray(data)) {
      for (const item of data) {
        const found = findReferencedObject(item, refId);
        if (found) {
          return found;
        }
      }
    } else if (typeof data === 'object' && data !== null) {
      for (const key in data) {
        const found = findReferencedObject(data[key], refId);
        if (found) {
          return found;
        }
      }
    }
    return undefined;
  };

  const getFullLeaseObject = (lease) => {
    if (lease?.$ref) {
      if (resolvedLeases[lease.$ref]) {
        return resolvedLeases[lease.$ref];
      }
      const resolved = findReferencedObject(fullLeaseData, lease.$ref);
      if (resolved) {
        setResolvedLeases(prev => ({ ...prev, [lease.$ref]: resolved }));
        return resolved;
      }
      return undefined;
    }
    return lease;
  };

  const getStatusDisplay = (status) => {
    switch (status) {
      case 'Pending':
        return <span className="status pending"><FaClock /> Pending</span>;
      case 'Accepted':
        return <span className="status accepted"><FaCheckCircle /> Accepted</span>;
      case 'Rejected':
        return <span className="status rejected"><FaTimesCircle /> Rejected</span>;
      default:
        return <span className="status">{status}</span>;
    }
  };

  const handleDeleteLease = async (lease) => {
    const fullLease = getFullLeaseObject(lease);
    if (fullLease && fullLease.status.toLowerCase() !== 'accepted') {
      toast.warn(
        `Are you sure you want to delete lease agreement ${fullLease.leaseID}?`,
        {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
          progress: undefined,
          closeButton: true,
          onClose: () => {
            // Do nothing if closed without action
          },
          onClick: async () => {
            setLoading(true);
            setDeletingLeaseId(fullLease.leaseID);
            setError('');
            const tenantToken = localStorage.getItem('tenantToken');

            if (!tenantToken) {
              setError('Authentication token not found.');
              setLoading(false);
              setDeletingLeaseId(null);
              toast.error('Authentication token not found.');
              return;
            }

            try {
              await axios.delete(
                `http://localhost:5162/api/LeaseAgreement/${fullLease.leaseID}`,
                {
                  headers: {
                    Authorization: `Bearer ${tenantToken}`,
                  },
                }
              );
              setLeases(leases.filter(l => getFullLeaseObject(l)?.leaseID !== fullLease.leaseID));
              toast.success('Lease agreement deleted successfully!');
            } catch (err) {
              console.error('Error deleting lease:', err);
              setError(err.response?.data || 'Failed to delete lease agreement.');
              toast.error(err.response?.data || 'Failed to delete lease agreement.');
            } finally {
              setLoading(false);
              setDeletingLeaseId(null);
            }
          },
        }
      );
    } else if (fullLease?.status.toLowerCase() === 'accepted') {
      toast.warn('Cannot delete an accepted lease agreement.');
    }
  };

  const handleDoPayment = (lease) => {
    const fullLease = getFullLeaseObject(lease);
    navigate(`/tenant/payment/form/${fullLease?.leaseID}`, { state: { lease: fullLease } });
  };

  if (loading) {
    return <div className="loading">Loading lease agreements...</div>;
  }

  if (error) {
    return <div className="error-message">{error}</div>;
  }

  const isGridLayout = leases.length >= 4;
  const leasesListClassName = `leases-list ${isGridLayout ? 'grid-layout' : 'single-column-layout'}`;

  return (
    <div className="leases-container">
      <h2>My Lease Agreements</h2>
      <ToastContainer position="top-right" autoClose={3000} hideProgressBar={false} newestOnTop={true} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
      {leases.length === 0 && !loading && <div className="no-leases"><p>You don't have any active leases yet.</p></div>}
      <div className={leasesListClassName}>
        {leases.map(lease => {
          const fullLease = getFullLeaseObject(lease);
          const propertyName = fullLease?.property?.propertyName || 'Property Details Unavailable';
          const startDate = fullLease?.startDate ? new Date(fullLease.startDate).toLocaleDateString() : 'Invalid Date';
          const endDate = fullLease?.endDate ? new Date(fullLease.endDate).toLocaleDateString() : 'Invalid Date';

          return (
            <div key={lease?.$id} className={`lease-card`}>
              <div className="lease-header">
                <FaFileContract className="lease-icon" />
                <h3>{propertyName}</h3>
              </div>

              <div className="lease-details">
                <p><FaCalendarAlt /> <strong>Term:</strong> {startDate} to {endDate}</p>
                {fullLease?.property?.rentAmount && <p><strong>Rent:</strong> ${fullLease.property.rentAmount}/month</p>}
                <p><strong>Status:</strong> {getStatusDisplay(fullLease?.status)}</p>
                <div className="expanded-details">
                  {fullLease?.property?.address && <p><strong>Address:</strong> {fullLease.property.address}, {fullLease.property.state}, {fullLease.property.country}</p>}
                  {fullLease?.property?.amenities && <p><strong>Amenities:</strong> {fullLease.property.amenities}</p>}
                  <p><strong>Lease ID:</strong> {fullLease?.leaseID}</p>
                  <p><strong>Property ID:</strong> {fullLease?.propertyID}</p>
                  <p><strong>Tenant ID:</strong> {fullLease?.tenantID}</p>
                  {fullLease?.tenantSignaturePath && <p><strong>Tenant Signature Path:</strong> {fullLease.tenantSignaturePath}</p>}
                  {fullLease?.tenantDocumentPath && <p><strong>Tenant Document Path:</strong> {fullLease.tenantDocumentPath}</p>}
                </div>
              </div>

              <div className="lease-actions">
                {fullLease?.status?.toLowerCase() !== 'accepted' && (
                  <button
                    className="btn-danger"
                    onClick={() => handleDeleteLease(lease)}
                    disabled={deletingLeaseId === fullLease?.leaseID}
                  >
                    {deletingLeaseId === fullLease?.leaseID ? 'Deleting...' : 'Delete'}
                  </button>
                )}
                {fullLease?.tenantDocumentPath && (
                  <a href={`/LeaseAgreements/LeaseAgreement.pdf`} target="_blank" rel="noopener noreferrer" className="btn-download">
                    <FaDownload /> View Lease Document
                  </a>
                )}
                {fullLease?.status === 'Accepted' && (
                  <button className="btn-primary" onClick={() => handleDoPayment(fullLease)}>
                    <FaMoneyBill /> Do Payment
                  </button>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

export default MyLeases;