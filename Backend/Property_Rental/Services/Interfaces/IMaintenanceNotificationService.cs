using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IMaintenanceNotificationService
    {
        Task AddNotificationAsync(int userId, string message);
        Task<List<MaintenanceNotification>> GetNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
    }
}
