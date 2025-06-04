using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public class LatePaymentNotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<LatePaymentNotificationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public LatePaymentNotificationBackgroundService(
            ILogger<LatePaymentNotificationBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Late Payment Notification Background Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<ILatePaymentNotificationService>();

                        // Get all executed leases and include related data
                        var executedLeases = dbContext.LeaseAgreements
                            .Include(la => la.Tenant)
                            .Include(la => la.Property)
                                .ThenInclude(p => p.Owner) // Ensure Owner is loaded
                            .Where(la => la.Status == "Executed")
                            .ToList();

                        foreach (var lease in executedLeases)
                        {
                            _logger.LogInformation($"Processing lease with ID: {lease.LeaseID}, Start Date: {lease.StartDate}");

                            var daysSinceStart = (DateTime.UtcNow - lease.StartDate).TotalDays;
                            var intervals = (int)(daysSinceStart / 30);
                            _logger.LogInformation($"Days Since Start: {daysSinceStart}, Intervals: {intervals}");

                            for (int i = 1; i <= intervals; i++)
                            {
                                var dueDate = lease.StartDate.AddDays(i * 30);
                                _logger.LogInformation($"Checking payments for interval {i}, Due Date: {dueDate}");

                                var paymentMade = dbContext.Payments
                                    .Any(p => p.LeaseID == lease.LeaseID &&
                                              p.PaymentDate >= dueDate.AddDays(-30) &&
                                              p.PaymentDate <= dueDate);

                                if (!paymentMade)
                                {
                                    _logger.LogInformation($"No payment made for Lease ID {lease.LeaseID}, Due Date: {dueDate}");

                                    var tenantMessage = $"Your payment for Lease ID {lease.LeaseID} (Due Date: {dueDate:yyyy-MM-dd}) is overdue. Please make the payment as soon as possible.";
                                    await notificationService.AddNotificationAsync(lease.Tenant.TenantID, lease.Property.Owner.OwnerID, tenantMessage);

                                    var ownerMessage = $"Tenant {lease.Tenant.Name} has not made the payment for Lease ID {lease.LeaseID} (Due Date: {dueDate:yyyy-MM-dd}).";
                                    await notificationService.AddNotificationAsync(lease.Tenant.TenantID, lease.Property.Owner.OwnerID, ownerMessage);

                                    _logger.LogInformation($"Late payment notifications sent for Lease ID {lease.LeaseID} (Due Date: {dueDate:yyyy-MM-dd}).");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing late payment notifications.");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}