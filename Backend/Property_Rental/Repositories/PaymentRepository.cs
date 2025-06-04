using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.LeaseAgreement)
                .FirstOrDefaultAsync(p => p.PaymentID == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.LeaseAgreement)
                .ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidatePaymentAmountAsync(int leaseId, double amount)
        {
            var lease = await _context.LeaseAgreements
                .Include(l => l.Property)
                .FirstOrDefaultAsync(l => l.LeaseID == leaseId);

            if (lease == null) return false;

            // Compare with tolerance for floating-point precision
            return Math.Abs(lease.Property.RentAmount - amount) < 0.001;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByLeaseAsync(int leaseId)
        {
            return await _context.Payments
                .Where(p => p.LeaseID == leaseId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}