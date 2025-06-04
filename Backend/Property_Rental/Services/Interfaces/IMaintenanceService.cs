using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<int> SubmitMaintenanceRequest(MaintenanceRequest maintenanceRequest);
        Task<List<MaintenanceRequest>> GetPendingRequestsForOwner(int ownerId);
        Task<bool> UpdateRequestStatus(int requestId, string status);
        Task<bool> GenerateBill(int serviceRequestId, double amount);
        Task<IEnumerable<MaintenanceRequest>> GetAllMaintenanceRequestsAsync();
        Task<MaintenanceRequestDTO?> GetMaintenanceRequestByIdAsync(int id);
        Task<MaintenanceRequest> CreateMaintenanceRequestAsync(MaintenanceRequestDTO maintenanceRequestDTO);
        Task<bool> UpdateMaintenanceRequestAsync(int id, MaintenanceRequestDTO maintenanceRequestDTO);
        Task<bool> DeleteMaintenanceRequestAsync(int id);
        Task AddOwnerNotificationAsync(MaintenanceNotification notification);
        Task<List<MaintenanceNotification>> GetOwnerNotificationsAsync(int ownerId);

        Task<bool> MarkNotificationAsReadAsync(int notificationId);
        Task<IEnumerable<MaintenanceRequestDTO>> GetRequestsByTenantIdAsync(int tenantId);
        Task AddTenantNotificationAsync(MaintenanceNotification notification);

    }
}