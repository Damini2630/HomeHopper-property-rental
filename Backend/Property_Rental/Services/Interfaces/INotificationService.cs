using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(int leaseId, string message);
        Task<List<Notification>> GetNotificationsByOwnerIdAsync(int ownerId);
        Task<List<Notification>> GetNotificationsByTenantIdAsync(int tenantId);
    }
}
