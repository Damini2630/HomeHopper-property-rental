import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import './RentalApplicationForm.css'; // Reuse the CSS

function EditRentalApplicationForm() {
    const { id } = useParams(); // Get the application ID from the route
    const navigate = useNavigate();
    const [applicationData, setApplicationData] = useState({
        propertyId: '',
        ownerId: '',
        noOfPeople: '',
        stayPeriod: '',
        tentativeStartDate: '',
        permanentAddress: '',
        state: '',
        country: '',
        specificRequirements: '',
        status: '', // Include status in the state
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState('');
    const [canEdit, setCanEdit] = useState(false);

    useEffect(() => {
        const fetchApplicationData = async () => {
            setLoading(true);
            setError('');
            try {
                const token = localStorage.getItem('tenantToken');
                const response = await fetch(`http://localhost:5162/api/RentalApplication/${id}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });

                if (response.ok) {
                    const data = await response.json();
                    setApplicationData(data);
                    setCanEdit(data.status.toLowerCase() === 'pending');
                } else if (response.status === 404) {
                    setError('Application not found.');
                } else {
                    const errorMessage = await response.text();
                    setError(`Failed to fetch application: ${errorMessage}`);
                }
            } catch (err) {
                setError('Error fetching application.');
            } finally {
                setLoading(false);
            }
        };

        fetchApplicationData();
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setApplicationData(prevData => ({
            ...prevData,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!canEdit) {
            setSubmitError('This application cannot be edited as its status is not "Pending".');
            return;
        }
        setSubmitting(true);
        setSubmitError('');

        try {
            const token = localStorage.getItem('tenantToken');
            const response = await fetch(`http://localhost:5162/api/RentalApplication/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                },
                body: JSON.stringify(applicationData),
            });

            if (response.ok) {
                navigate('/tenant/applications'); // Redirect after successful update
            } else {
                const errorMessage = await response.text();
                setSubmitError(`Failed to update application: ${errorMessage}`);
            }
        } catch (err) {
            setSubmitError('Error updating application.');
        } finally {
            setSubmitting(false);
        }
    };

    if (loading) {
        return <div>Loading application details...</div>;
    }

    if (error) {
        return <div className="error-message">{error}</div>;
    }

    return (
        <div className="rental-application-form">
            <h2>Edit Rental Application</h2>
            {!canEdit && (
                <div className="error-message">
                    This application has a status of "{applicationData.status}". You can only edit applications with a "Pending" status.
                </div>
            )}
            {submitError && <div className="error-message">{submitError}</div>}
            <form onSubmit={handleSubmit}>
                {/* Form fields - similar to Create form, but pre-filled */}
                <div className="form-group">
                    <label htmlFor="propertyId">Property ID:</label>
                    <input type="number" id="propertyId" name="propertyId" value={applicationData.propertyId} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="ownerId">Owner ID:</label>
                    <input type="number" id="ownerId" name="ownerId" value={applicationData.ownerId} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="noOfPeople">Number of People:</label>
                    <input type="number" id="noOfPeople" name="noOfPeople" value={applicationData.noOfPeople} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="stayPeriod">Preferred Stay Period:</label>
                    <input type="text" id="stayPeriod" name="stayPeriod" value={applicationData.stayPeriod} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="tentativeStartDate">Tentative Start Date:</label>
                    <input type="date" id="tentativeStartDate" name="tentativeStartDate" value={applicationData.tentativeStartDate ? applicationData.tentativeStartDate.split('T')[0] : ''} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="permanentAddress">Permanent Address:</label>
                    <textarea id="permanentAddress" name="permanentAddress" value={applicationData.permanentAddress} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="state">State:</label>
                    <input type="text" id="state" name="state" value={applicationData.state} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="country">Country:</label>
                    <input type="text" id="country" name="country" value={applicationData.country} onChange={handleChange} required disabled={!canEdit} />
                </div>
                <div className="form-group">
                    <label htmlFor="specificRequirements">Specific Requirements (Optional):</label>
                    <textarea id="specificRequirements" name="specificRequirements" value={applicationData.specificRequirements} onChange={handleChange} disabled={!canEdit} />
                </div>

                <button type="submit" className="submit-button" disabled={submitting || !canEdit}>
                    {submitting ? 'Updating...' : 'Update Application'}
                </button>
            </form>
            <div className="navigation-links">
                <Link to="/tenant/applications">Back to Applications</Link>
                <Link to="/tenant">Go to Dashboard</Link>
            </div>
        </div>
    );
}

export default EditRentalApplicationForm;