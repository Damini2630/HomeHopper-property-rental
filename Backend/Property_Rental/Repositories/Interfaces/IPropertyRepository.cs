using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property> GetPropertyByIdAsync(int id);
        Task<Property> GetPropertyByNameAsync(string name);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(int ownerId); // Parameter is now int
        Task<IEnumerable<Property>> SearchPropertiesAsync(string state, string country);
        Task<Property> AddPropertyAsync(Property property);
        Task<bool> UpdatePropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int id);
    }
}