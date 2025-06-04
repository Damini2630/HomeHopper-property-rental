using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(int paymentId);
        Task<bool> ValidatePaymentAmountAsync(int leaseId, double amount);
        Task<IEnumerable<Payment>> GetPaymentsByLeaseAsync(int leaseId);
    }
}