using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant> AddTenantAsync(TenantRegistrationDTO tenantRegistrationDTO);
        Task<string> LoginTenantAsync(TenantLoginDTO tenantLoginDTO);
        Task<Tenant> GetTenantByEmailAsync(string email);
        Task<List<LeaseAgreement>> GetLeaseAgreementsAsync(int tenantId);
        Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync(int tenantId);
        //Task<List<string>> CheckPaymentStatusAsync(int tenantId);
        Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int tenantId);
        Task<string> CheckApplicationStatusAsync(int applicationId);

        Task<bool> ResetPasswordAsync(string email, string newPassword);

        //Task<bool> UpdateTenantAsync(int tenantId, TenantUpdateDTO tenantUpdateDTO);
    }
}