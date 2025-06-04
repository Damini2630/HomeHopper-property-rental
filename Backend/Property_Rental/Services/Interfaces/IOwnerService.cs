using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IOwnerService
    {
        Task<Owner> AddOwnerAsync(OwnerRegistrationDTO ownerRegistrationDTO);
        Task<string> LoginOwnerAsync(OwnerLoginDTO ownerLoginDTO);
        Task<Owner> GetOwnerByEmailAsync(string email);
        Task<Owner> GetOwnerByIdAsync(int ownerId);
        Task<List<LeaseAgreement>> GetLeaseAgreementsAsync(int ownerId);

        Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync(int ownerId);
        Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int ownerId);
        //Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId);
        Task<bool> ResetPasswordAsync(string email, string newpassword);
        Task<bool> UpdateOwnerAsync(int ownerId, OwnerUpdateDTO ownerUpdateDTO);
        Task<bool> DeleteOwnerAsync(int ownerId);

    }
}
