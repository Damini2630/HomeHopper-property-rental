using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Repositories
{


    public interface IRentalApplicationRepository
    {
        Task<bool> AddRentalApplicationAsync(RentalApplication rentalApplication);
        Task<IEnumerable<RentalApplication>> GetAllRentalApplicationsAsync();
        Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId);
        Task<List<RentalApplication>> GetRentalApplicationsByOwnerIdAsync(int ownerId);
        Task<List<RentalApplication>> GetRentalApplicationsByTenantIdAsync(int tenantId);
        Task<RentalApplication> GetRentalApplicationByIdAsync(int id);
        Task<bool> UpdateRentalApplicationStatusAsync(int id, string status);
        Task<bool> UpdateRentalApplicationAsync(RentalApplication rentalApplication); // Add this to the interface
        Task<bool> DeleteRentalApplicationAsync(int id); // Add this to the interface
    }
}