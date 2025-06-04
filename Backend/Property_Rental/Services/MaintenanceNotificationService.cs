using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;

namespace OnlineRentalPropertyManagement.Services
{
    public class MaintenanceNotificationService : IMaintenanceNotificationService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(int tenantId, string message)
        {
            var notification = new MaintenanceNotification
            {
                tenantid = tenantId,
                Message = message
            };

            _context.MaintenanceNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MaintenanceNotification>> GetNotificationsAsync(int tenantid)
        {
            return await _context.MaintenanceNotifications
                .Where(n => n.tenantid == tenantid && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.MaintenanceNotifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}

