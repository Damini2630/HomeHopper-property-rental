using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ApplicationDbContext context)
        {
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            // Auto-generate invoice number if not set
            if (string.IsNullOrEmpty(payment.InvoiceNumber))
            {
                payment.InvoiceNumber = GenerateInvoiceNumber();
            }

            await _paymentRepository.AddPaymentAsync(payment);

            // Mark related late notifications as resolved
            await ResolveLateNotifications(payment);
        }

        public async Task<bool> ValidatePaymentAmountAsync(int leaseId, double amount)
        {
            var lease = await _context.LeaseAgreements
                .Include(l => l.Property)
                .FirstOrDefaultAsync(l => l.LeaseID == leaseId);

            return lease != null && Math.Abs(lease.Property.RentAmount - amount) < 0.001; // Floating-point tolerance
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _paymentRepository.GetPaymentByIdAsync(paymentId);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllPaymentsAsync();
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            await _paymentRepository.UpdatePaymentAsync(payment);
        }

        public async Task DeletePaymentAsync(int paymentId)
        {
            await _paymentRepository.DeletePaymentAsync(paymentId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByTenantAsync(int tenantId)
        {
            return await _context.Payments
                .Include(p => p.LeaseAgreement)
                .Where(p => p.LeaseAgreement.TenantID == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOwnerAsync(int ownerId)
        {
            return await _context.Payments
                .Include(p => p.LeaseAgreement)
                    .ThenInclude(l => l.Property)
                .Where(p => p.LeaseAgreement.Property.OwnerID == ownerId)
                .ToListAsync();
        }

        private async Task ResolveLateNotifications(Payment payment)
        {
            var lease = await _context.LeaseAgreements
                .FirstOrDefaultAsync(l => l.LeaseID == payment.LeaseID);

            if (lease != null)
            {
                var notifications = await _context.LatePaymentNotifications
                    .Where(n => n.TenantID == lease.TenantID &&
                               n.PropertyID == lease.PropertyID &&
                               !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }
        }

        private static string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }
    }
}