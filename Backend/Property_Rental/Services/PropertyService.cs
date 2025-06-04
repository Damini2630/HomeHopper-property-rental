using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<Property>> GetPropertiesAsync()
        {
            return await _propertyRepository.GetAllPropertiesAsync();
        }
        public async Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(int ownerId)
        {
            return await _propertyRepository.GetPropertiesByOwnerIdAsync(ownerId);
        }
        public async Task<Property> GetPropertyAsync(int id)
        {
            return await _propertyRepository.GetPropertyByIdAsync(id);
        }

        

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string state, string country)
        {
            return await _propertyRepository.SearchPropertiesAsync(state, country);
        }

        public async Task<Property> AddPropertyAsync(Property property) // Accepts Property entity directly
        {
            return await _propertyRepository.AddPropertyAsync(property);
        }

        public async Task<bool> UpdatePropertyAsync(Property existingProperty) // Accepts Property entity directly
        {
            return await _propertyRepository.UpdatePropertyAsync(existingProperty);
        }
        public async Task<bool> UpdatePropertyAsync(int id, PropertyDTO propertyDTO)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(id);
            if (property == null)
                return false;

            property.PropertyName = propertyDTO.PropertyName;
            property.Address = propertyDTO.Address;
            property.State = propertyDTO.State;
            property.Country = propertyDTO.Country;
            property.RentAmount = propertyDTO.RentAmount;
            property.AvailabilityStatus = propertyDTO.AvailabilityStatus;
            property.Amenities = propertyDTO.Amenities;
            property.OwnerID = propertyDTO.OwnerID;

            return await _propertyRepository.UpdatePropertyAsync(property);
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            return await _propertyRepository.DeletePropertyAsync(id);
        }
    }
}