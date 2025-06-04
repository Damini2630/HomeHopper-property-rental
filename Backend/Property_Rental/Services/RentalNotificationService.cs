using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;

namespace OnlineRentalPropertyManagement.Services
{
    public class RentalNotificationService : IRentalNotificationService
    {
        private readonly ApplicationDbContext _context;
        public RentalNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public Task AddNotificationAsync(int userId, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToListAsync();
        }

        public async Task NotifyUserAsync(int propertyId, string message)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property != null)
            {
                var notification = new Notification
                {
                    UserID = property.OwnerID,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                await AddNotificationAsync(notification);
            }
        }
    }
}
