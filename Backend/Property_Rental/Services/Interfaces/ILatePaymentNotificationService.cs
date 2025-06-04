using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface ILatePaymentNotificationService
    {
        Task MarkAsReadAsync(int notificationId);
        Task<List<string>> GetLatePaymentNotificationsForOwnerAsync(int ownerId);
        Task<List<(string PropertyName, bool IsDue)>> CheckPaymentStatusForTenantAsync(int tenantId);
        Task<List<string>> GetLatePaymentNotificationsForTenantAsync(int tenantId);
        Task AddNotificationAsync(int tenantId, int ownerId, string message); // Updated method
    }
}