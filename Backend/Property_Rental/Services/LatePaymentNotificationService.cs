using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public class LatePaymentNotificationService : ILatePaymentNotificationService
    {
        private readonly ApplicationDbContext _context;

        public LatePaymentNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(int tenantId, int ownerId, string message)
        {
            var notification = new LatePaymentNotification
            {
                TenantID = tenantId,
                OwnerID = ownerId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            try
            {
                _context.LatePaymentNotifications.Add(notification);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Notification added: TenantID={tenantId}, OwnerID={ownerId}, Message={message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding notification: {ex.Message}");
            }

        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.LatePaymentNotifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetLatePaymentNotificationsForOwnerAsync(int ownerId)
        {
            var notifications = await _context.LatePaymentNotifications
                .Include(n => n.Tenant)
                .Include(n => n.Property)
                .Where(n => n.OwnerID == ownerId)
                .ToListAsync();

            return notifications.Select(n =>
                $"The tenant with tenant ID: {n.TenantID}, and name: {n.Tenant.Name} for property with ID: {n.Property.PropertyID}, and name: {n.Property.PropertyName}, is due for payment."
            ).ToList();
        }

        public async Task<List<string>> GetLatePaymentNotificationsForTenantAsync(int tenantId)
        {
            var notifications = await _context.LatePaymentNotifications
                .Include(n => n.Property)
                .Where(n => n.TenantID == tenantId)
                .ToListAsync();

            return notifications.Select(n =>
                $"You have a late payment for property with ID: {n.Property.PropertyID}, and name: {n.Property.PropertyName}."
            ).ToList();
        }

        public async Task<List<(string PropertyName, bool IsDue)>> CheckPaymentStatusForTenantAsync(int tenantId)
        {
            var leaseAgreements = await _context.LeaseAgreements
                .Include(la => la.Property)
                .Where(la => la.TenantID == tenantId && la.Status == "Active")
                .ToListAsync();

            var paymentStatus = new List<(string PropertyName, bool IsDue)>();

            foreach (var lease in leaseAgreements)
            {
                var daysSinceStart = (DateTime.UtcNow - lease.StartDate).TotalDays;
                var intervals = (int)(daysSinceStart / 30);

                var isDue = false;
                for (int i = 1; i <= intervals; i++)
                {
                    var dueDate = lease.StartDate.AddDays(i * 30);
                    var paymentMade = await _context.Payments
                        .AnyAsync(p => p.LeaseID == lease.LeaseID &&
                                       p.PaymentDate >= dueDate.AddDays(-30) &&
                                       p.PaymentDate <= dueDate);

                    if (!paymentMade)
                    {
                        isDue = true;
                        break;
                    }
                }

                paymentStatus.Add((lease.Property.PropertyName, isDue));
            }

            return paymentStatus;
        }
    }
}