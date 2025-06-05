import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './Payments.css'; // Create a CSS file for owner payments if needed

const OwnerPaymentDetails = () => {
  const [paymentDetails, setPaymentDetails] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const token = localStorage.getItem('ownerToken');

  useEffect(() => {
    const fetchOwnerPaymentDetails = async () => {
      setLoading(true);
      setError('');
      try {
        const response = await axios.get('http://localhost:5162/api/Payments/owner/details', {
          headers: { Authorization: `Bearer ${token}` },
        });
        console.log("Owner Payment Details:", response.data);
        setPaymentDetails(response.data?.$values || []);
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to fetch payment details.');
      } finally {
        setLoading(false);
      }
    };

    fetchOwnerPaymentDetails();
  }, [token]);

  if (loading) {
    return <div>Loading payment details...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="owner-payments-container">
      <h2>Payment Details</h2>
      {paymentDetails.length > 0 ? (
        <table>
          <thead>
            <tr>
              <th>Invoice Number</th>
              <th>Property Name</th>
              <th>Tenant Name</th>
              <th>Amount Due</th>
              <th>Payment Date</th>
              <th>Payment Method</th>
              <th>Status</th>
              {/* Add more relevant columns based on your API response */}
            </tr>
          </thead>
          <tbody>
            {paymentDetails.map(payment => (
              <tr key={payment.invoiceNumber}>
                <td>{payment.invoiceNumber}</td>
                <td>{payment.propertyName}</td>
                <td>{payment.tenantName}</td>
                <td>₹{payment.amountDue}</td>
                <td>{payment.paymentDate ? new Date(payment.paymentDate).toLocaleDateString('en-IN') : 'N/A'}</td>
                <td>{payment.paymentMethod || 'N/A'}</td>
                <td className={`status ${payment.status?.toLowerCase()}`}>
                  {payment.status || 'Paid'}
                </td>
                {/* Add more relevant data cells */}
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No payment details available.</p>
      )}
    </div>
  );
};

export default OwnerPaymentDetails;