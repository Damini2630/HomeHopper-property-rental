using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IRentalNotificationService
    {
        Task AddNotificationAsync(int userId, string message);
        Task<List<Notification>> GetUnreadNotificationsAsync(int userId);
        Task NotifyUserAsync(int propertyId, string message);
    }
}
