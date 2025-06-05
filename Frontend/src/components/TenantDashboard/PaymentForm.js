import React, { useState } from 'react';
import axios from 'axios';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import './PaymentForm.css';

const PaymentForm = () => {
    const { notificationID } = useParams(); // You might still have this if payments are also triggered from notifications
    const navigate = useNavigate();
    const location = useLocation();
    const lease = location.state?.lease; // Access the lease data passed from MyLeases

    const [paymentDetails, setPaymentDetails] = useState({
        leaseID: lease?.leaseID || '', // Pre-fill leaseID if available
        amount: '', // You might want to pre-fill this from lease.property.rentAmount
        paymentMethod: 'CreditCard',
    });
    const [paymentError, setPaymentError] = useState('');
    const [paymentSuccess, setPaymentSuccess] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setPaymentDetails(prevState => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setPaymentError('');
        setPaymentSuccess(false);

        try {
            const token = localStorage.getItem('tenantToken');
            const paymentDto = {
                leaseID: parseInt(paymentDetails.leaseID, 10),
                amount: parseFloat(paymentDetails.amount),
                paymentMethod: paymentDetails.paymentMethod,
            };
            console.log("Payload being sent:", paymentDto);
            const response = await axios.post('http://localhost:5162/api/Payments', paymentDto, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });
            setPaymentSuccess(true);
            setTimeout(() => {
                navigate('/tenant/payments');
            }, 2000);
        } catch (error) {
            setPaymentError(error.response?.data?.errors?.PaymentMethod?.[0] || error.response?.data?.title || 'Failed to process payment.');
        }
    };

    return (
        <div className="payment-form-container">
            <h2>Make Payment</h2>
            {notificationID && <p className="notification-info">Paying for Notification ID: {notificationID}</p>}
            {lease?.property?.propertyName && <p className="property-info">For Property: {lease.property.propertyName}</p>}
            {paymentSuccess && <p className="success-message">Payment successful!</p>}
            {paymentError && <p className="error-message">{paymentError}</p>}
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="leaseID">Lease ID:</label>
                    <input
                        type="number"
                        id="leaseID"
                        name="leaseID"
                        value={paymentDetails.leaseID}
                        onChange={handleChange}
                        required
                        className="form-control"
                        readOnly // Make it read-only if pre-filled from lease
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="amount">Amount:</label>
                    <input
                        type="number"
                        id="amount"
                        name="amount"
                        value={paymentDetails.amount}
                        onChange={handleChange}
                        required
                        className="form-control"
                        // You might want to set a default value here based on lease.property.rentAmount
                        // defaultValue={lease?.property?.rentAmount || ''}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="paymentMethod">Payment Method:</label>
                    <select
                        id="paymentMethod"
                        name="paymentMethod"
                        value={paymentDetails.paymentMethod}
                        onChange={handleChange}
                        required
                        className="form-control"
                    >
                        <option value="CreditCard">Credit Card</option>
                        <option value="DebitCard">Debit Card</option>
                        <option value="BankTransfer">Bank Transfer</option>
                        {/* Add more payment methods with PascalCase values as needed */}
                    </select>
                </div>
                <button type="submit" className="pay-button">Pay Now</button>
                <button type="button" onClick={() => navigate('/tenant/payments')} className="cancel-button">Cancel</button>
            </form>
        </div>
    );
};

export default PaymentForm;