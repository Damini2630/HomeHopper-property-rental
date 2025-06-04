using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IRentalApplicationService
    {
        Task<bool> SubmitRentalApplicationAsync(RentalApplicationDTO rentalApplicationDTO);
        Task<IEnumerable<RentalApplication>> GetAllRentalApplicationsAsync();
        Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId);
        Task<List<RentalApplicationDTO>> GetRentalApplicationsByOwnerIdAsync(int ownerId);
        Task<List<RentalApplication>> GetRentalApplicationsByTenantIdAsync(int tenantId);
        Task<RentalApplication> GetRentalApplicationByIdAsync(int id);
        Task<bool> UpdateRentalApplicationStatusAsync(int id, string status);
        Task<bool> UpdateRentalApplicationAsync(RentalApplicationDTO rentalApplicationDTO);
        Task<bool> DeleteRentalApplicationAsync(int id);
    }
}