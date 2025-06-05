import React, { useState } from 'react';
import { FaMapMarkerAlt, FaPhone, FaEnvelope, FaClock } from 'react-icons/fa';
import { motion } from 'framer-motion';
import './Contact.css';

const Contact = () => {
  return (
    <motion.section
      className="contact"
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.5 }}
    >
      <div className="contact-header">
        <h2>Contact HomeHopper</h2>
        <p>We're here to help with your rental journey</p>
      </div>

      <div className="contact-container">
        <motion.div
          className="contact-info"
          initial={{ x: -50 }}
          animate={{ x: 0 }}
        >
          <div className="info-card">
            <FaMapMarkerAlt className="icon" />
            <h3>Our Headquarters</h3>
            <p>Cognizant Technology Solutions<br />Chennai, Siruseri, TN 603103</p>
            <a
              href="https://maps.google.com"
              target="_blank"
              rel="noopener noreferrer"
              className="map-link"
            >
              View on Map
            </a>
          </div>

          

          <div className="info-card">
            <FaEnvelope className="icon" />
            <h3>Email Us</h3>
            <p>General: <a href="mailto:hello@homehopper.com">hello@homehopper.com</a></p>
            <p>Support: <a href="mailto:support@homehopper.com">support@homehopper.com</a></p>
          </div>

          
        </motion.div>

        <div className="map-container">
          <iframe
            title="HomeHopper Location"
            src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3912.238104735234!2d80.2098243152605!3d12.832825140365768!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3a525d1d1d1d1d1d%3A0x1d1d1d1d1d1d1d1d!2sSiruseri%2C%20Tamil%20Nadu%20603103!5e0!3m2!1sen!2sin!4v1620000000000!5m2!1sen!2sin"
            width="100%"
            height="100%"
            style={{ border: 0 }}
            allowFullScreen=""
            loading="lazy"
          ></iframe>
        </div>
      </div>
    </motion.section>
  );
};

export default Contact;