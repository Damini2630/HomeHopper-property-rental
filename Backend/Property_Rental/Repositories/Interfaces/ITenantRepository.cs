using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
//using OnlineRentalPropertyManagement.Models.OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface ITenantRepository
    {
        Task<bool> AddTenantAsync(Tenant tenant);
        Task<Tenant> GetTenantByEmailAsync(string email);
        Task<List<LeaseAgreement>> GetLeaseAgreementsByTenantIdAsync(int tenantId);
        Task<List<MaintenanceRequest>> GetMaintenanceRequestsByTenantIdAsync(int tenantId);
        //Task<List<LatePaymentNotification>> CheckPaymentStatusForTenantAsync(int tenantId);
        Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int tenantId);
        Task<List<RentalApplication>> CheckApplicationStatusAsync(int applicationId);


        //already exisits------Task<Tenant> GetTenantByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<bool> UpdateTenantAsync(Tenant tenant);



        //Task<bool> ResetPasswordAsync(string email, string newPassword);
        //Task<Tenant> GetTenantByIdAsync(int tenantId);

        //Task<bool> UpdateTenantAsync(Tenant, tenant);

        //Task<Tenant> GetTenantByEmailAsync(string email);
    }
}