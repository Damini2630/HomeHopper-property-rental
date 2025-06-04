using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetPropertiesAsync();
        Task<Property> GetPropertyAsync(int id);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(int ownerId);
        Task<IEnumerable<Property>> SearchPropertiesAsync(string state, string country);
        Task<Property> AddPropertyAsync(Property property); // Accepts Property entity
        Task<bool> UpdatePropertyAsync(Property existingProperty); // Accepts Property entity
        Task<bool> DeletePropertyAsync(int id);
        Task<bool> UpdatePropertyAsync(int id, PropertyDTO propertyDTO);
    }
}