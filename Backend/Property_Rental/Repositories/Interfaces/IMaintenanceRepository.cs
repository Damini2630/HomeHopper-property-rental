using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task<IEnumerable<MaintenanceRequest>> GetAllAsync();
        Task<MaintenanceRequest> GetByIdAsync(int id);
        Task<MaintenanceRequest> SubmitMaintenanceRequest(MaintenanceRequest maintenanceRequest);
        Task<bool> UpdateAsync(MaintenanceRequest maintenanceRequest);
        Task<bool> DeleteAsync(int id);
        Task AddMaintenanceNotificationAsync(MaintenanceNotification notification);

        // NEW: Get notifications for owner

        Task<List<MaintenanceNotification>> GetOwnerNotificationsAsync(int ownerId);

        // NEW: Mark notification as read

        Task<bool> MarkNotificationAsReadAsync(int notificationId);
        Task<IEnumerable<MaintenanceRequest>> GetRequestsByTenantIdAsync(int tenantId);


    }
}
