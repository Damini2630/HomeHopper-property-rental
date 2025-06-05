import React, { useState, useEffect, useContext } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import PropertyCard from '../shared/PropertyCard';
import './TenantDashboard.css';
import { useAppliedProperties } from './TenantLayout'; // Import the context
 
function PropertySearch() {
    const navigate = useNavigate();
    const [properties, setProperties] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchParams, setSearchParams] = useState({
        propertyName: '',
        // state: '',
        // country: ''
    });
    const [filteredProperties, setFilteredProperties] = useState([]);
    const appliedPropertyIds = useAppliedProperties(); // Access appliedPropertyIds from context
 
    useEffect(() => {
        const fetchAllProperties = async () => {
            setLoading(true);
            setError(null);
            try {
                const token = localStorage.getItem('tenantToken');
                const response = await axios.get('http://localhost:5162/api/property/all', {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });
                console.log('API Response:', response.data);
 
                if (response.data && Array.isArray(response.data.$values)) {
                    setProperties(response.data.$values);
                    setFilteredProperties(response.data.$values);
                } else {
                    setError('Error: Received unexpected data structure from the API.');
                }
            } catch (err) {
                console.error('Error fetching properties:', err);
                setError('Failed to load properties.');
            } finally {
                setLoading(false);
            }
        };
 
        fetchAllProperties();
    }, []);
 
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setSearchParams(prev => ({
            ...prev,
            [name]: value,
        }));
    };
 
    const handleSearch = async (e) => {
        e.preventDefault();
 
        const { propertyName } = searchParams;
 
        if (!propertyName.trim()) {
            // If empty, reset to all properties
            setFilteredProperties(properties);
            return;
        }
 
        try {
            const token = localStorage.getItem('tenantToken');
            const response = await axios.get(`http://localhost:5162/api/property/search/${propertyName}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            const data = response.data?.$values || response.data || [];
            setFilteredProperties(data);
        } catch (error) {
            console.error("Error searching properties:", error);
            setFilteredProperties([]);
        }
    };
 
 
    const handleApplyNow = (property) => {
        navigate(`/tenant/apply/${property.propertyID}`);
    };
 
    // const goToGeneralApplication = () => {
    //     navigate('/tenant/apply');
    // };
 
    if (loading) {
        return <div>Loading properties...</div>;
    }
 
    if (error) {
        return <div className="error-message">Error: {error}</div>;
    }
 
    return (
        <div className="property-list-tenant-container">
            {/* <div className="apply-now-button-top-right">
                <button onClick={goToGeneralApplication}>Apply for Rent</button>
            </div> */}
            <h2>Available Properties</h2>
            <br/>
 
            <div className='search-bar-wrapper'>
            <form className="search-bar" onSubmit={handleSearch}>
                <input
                    type="text"
                    name="propertyName"
                    placeholder="Search by Property Name"
                    value={searchParams.propertyName}
                    onChange={handleInputChange}
                />
                <button type="submit">Search</button>
            </form>
            </div>
 
 
            <div className="property-grid">
                {filteredProperties.map(property => (
                    <PropertyCard
                        key={property.PropertyID} // Assuming PropertyID is the unique key
                        property={property}
                        showApplyButton={true}
                        onApplyNow={handleApplyNow}
                        isTenantView={true}
                        appliedPropertyIds={appliedPropertyIds} // Pass the applied IDs
                    />
                ))}
            </div>
        </div>
    );
}
 
export default PropertySearch;