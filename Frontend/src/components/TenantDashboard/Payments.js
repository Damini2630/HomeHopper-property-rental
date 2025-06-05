import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './Payments.css';
// import './LatePaymentNotifications.css'; // Remove this import
import { useNavigate } from 'react-router-dom';

const Payments = () => {
    const [paymentHistory, setPaymentHistory] = useState([]);
    // const [lateNotifications, setLateNotifications] = useState([]); // Remove this state
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const token = localStorage.getItem('tenantToken');

    const fetchPaymentsAndNotifications = async () => {
        setLoading(true);
        setError('');
        try {
            const historyResponse = await axios.get('http://localhost:5162/api/Payments/history/tenant', {
                headers: { Authorization: `Bearer ${token}` },
            });
            console.log("Payment History Data:", historyResponse.data);
            const paymentsWithStatus = historyResponse.data?.$values?.map(payment => ({
                ...payment,
                status: 'Paid',
            })) || [];
            setPaymentHistory(paymentsWithStatus);

            // Remove the late payment notifications fetch
            // const notificationsResponse = await axios.get('http://localhost:5162/api/Payments/notifications/tenant', {
            //     headers: { Authorization: `Bearer ${token}` },
            // });
            // console.log("Late Payment Notifications:", notificationsResponse.data);
            // setLateNotifications(notificationsResponse.data?.$values || []);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch data.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPaymentsAndNotifications();
    }, []);

    // Remove the handlePayNowClick function
    // const handlePayNowClick = (notification) => {
    //     // Immediately remove the clicked notification from the state
    //     setLateNotifications(prevNotifications =>
    //         prevNotifications.filter(n => n.notificationID !== notification.notificationID)
    //     );
    //     navigate(`/tenant/payment/form/${notification.notificationID}`, { state: { notification } });
    // };

    if (loading) {
        return <div>Loading payment information...</div>;
    }

    if (error) {
        return <div className="error">{error}</div>;
    }

    return (
        <div className="payments-container">
            <h2>Payment History</h2>
            {paymentHistory.length > 0 ? (
                <table>
                    <thead>
                        <tr>
                            <th>Invoice Number</th>
                            <th>Amount</th>
                            <th>Payment Date</th>
                            <th>Payment Method</th>
                            <th>Property</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {paymentHistory.map(payment => (
                            <tr key={payment.invoiceNumber}>
                                <td>{payment.invoiceNumber}</td>
                                <td>₹{payment.amount}</td>
                                <td>{new Date(payment.paymentDate).toLocaleDateString('en-IN')}</td>
                                <td>{payment.paymentMethod}</td>
                                <td>{payment.propertyName}</td>
                                <td className={`status ${payment.status.toLowerCase()}`}>{payment.status}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                <p>No payment history available.</p>
            )}

            {/* Remove the Late Payment Notifications section entirely */}
            {/*
            <div className="late-payment-notifications-container" >
                <h2>Late Payment Notifications</h2>
                {lateNotifications.length > 0 ? (
                    <ul>
                        {lateNotifications.map(notification => (
                            <li key={notification.notificationID} className={notification.isPaid ? 'paid-notification' : ''}>
                                <p><strong>Property Name:</strong> {notification.propertyName}</p>
                                <p><strong>Property Address:</strong> {notification.propertyAddress}</p>
                                <p><strong>Message:</strong> {notification.message}</p>
                                <p><strong>Received On:</strong> {new Date(notification.createdAt).toLocaleDateString('en-IN')}</p>
                                {!notification.isPaid && (
                                    <button onClick={() => handlePayNowClick(notification)}>Pay Now</button>
                                )}
                                {notification.isPaid && (
                                    <button className="paid-button" disabled>Paid</button>
                                )}
                            </li>
                        ))}
                    </ul>
                ) : (
                    <p className="no-notifications">No late payment notifications at this time.</p>
                )}
            </div>
            */}
        </div>
    );
};

export default Payments;