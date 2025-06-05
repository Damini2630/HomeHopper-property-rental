import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axios from 'axios';
import './OwnerDashboard.css';

function AcceptLeaseAgreement() {
    const navigate = useNavigate();
    const { leaseId } = useParams();
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');
    const [successMessage, setSuccessMessage]=useState('');

    const handleAcceptClick = async () => {
        setSubmitting(true);
        setError('');
        setSuccessMessage('');

        const ownerToken = localStorage.getItem('ownerToken');

        if (!ownerToken) {
            setError('Owner token not found. Please log in.');
            setSubmitting(false);
            return;
        }

        console.log('Accept button clicked for Lease ID:', leaseId);

        try {
            const statusResponse = await axios.put(
                `http://localhost:5162/api/LeaseAgreement/${leaseId}/owner/status`,
                { action: 'accept' },
                {
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${ownerToken}`,
                    },
                }
            );

            console.log('✅ Lease status updated for Lease ID:', leaseId, statusResponse.data);
            setSuccessMessage(`Lease agreement for ID ${leaseId} accepted successfully!`);
            setSubmitting(false);
            // Navigate back to the leases page. The useEffect there will re-fetch.
            navigate('/owner/leases');
        } catch (error) {
            console.error('❌ Failed to accept agreement for Lease ID:', leaseId, error);
            setError(`Failed to accept the agreement for Lease ID: ${leaseId}. Please try again.`);
            if (error.response && error.response.data) {
                console.error('Server Error:', error.response.data);
                setError(error.response.data);
            } else {
                console.error('An unexpected error occurred:', error);
            }
            setSubmitting(false);
        }
    };

    return (
        <div className="accept-lease-container">
            <h2>Accept Lease Agreement</h2>
            {error && <div className="error-message">{error}</div>}
            {successMessage && <div className="success-message">{successMessage}</div>}
            <p>Are you sure you want to accept the lease agreement with ID: <strong>{leaseId}</strong>?</p>
            <div className="action-buttons">
                <button className="submit-button" onClick={handleAcceptClick} disabled={submitting}>
                    {submitting ? 'Accepting...' : 'Accept'}
                </button>
                <button type="button" onClick={() => navigate(-1)} className="back-button">
                    Back to Lease Agreements
                </button>
            </div>
        </div>
    );
}

export default AcceptLeaseAgreement;