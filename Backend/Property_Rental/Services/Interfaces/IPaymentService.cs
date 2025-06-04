using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IPaymentService
    {
        Task AddPaymentAsync(Payment payment);
        Task<bool> ValidatePaymentAmountAsync(int leaseId, double amount);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task UpdatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByTenantAsync(int tenantId);
        Task<IEnumerable<Payment>> GetPaymentsByOwnerAsync(int ownerId);
    }
}