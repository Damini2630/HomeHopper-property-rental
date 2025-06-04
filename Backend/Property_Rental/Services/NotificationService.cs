using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(int leaseId, string message)
        {
            var notification = new Notification
            {
                LeaseID = leaseId,
                Message = message,
                DateCreated = DateTime.UtcNow
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsByOwnerIdAsync(int ownerId)
        {
            return await _context.Notification
                .Where(n => n.LeaseAgreement.Property.OwnerID == ownerId)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetNotificationsByTenantIdAsync(int tenantId)
        {
            return await _context.Notification
                .Where(n => n.LeaseAgreement.TenantID == tenantId)
                .ToListAsync();
        }
    }
}