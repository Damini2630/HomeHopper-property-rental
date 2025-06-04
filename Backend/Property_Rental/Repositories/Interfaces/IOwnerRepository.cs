using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface IOwnerRepository
    {
        Task<bool> AddOwnerAsync(Owner owner);
        Task<Owner> GetOwnerByEmailAsync(string email);
        Task<Owner> GetOwnerByIdAsync(int ownerId);
        Task<bool> UpdateOwnerAsync(Owner owner);
        Task<List<LeaseAgreement>> GetLeaseAgreementsByOwnerIdAsync(int ownerId);
        Task<List<MaintenanceRequest>> GetMaintenanceRequestsByOwnerIdAsync(int ownerId);
        Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int ownerId);
        Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId);

        Task<bool> ResetPasswordAsync(string email, string newPassword);
     

        Task<bool> DeleteOwnerAsync(int ownerId);


    }
}

