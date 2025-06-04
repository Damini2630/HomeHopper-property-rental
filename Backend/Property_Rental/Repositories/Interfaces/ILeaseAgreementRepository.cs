using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface ILeaseAgreementRepository
    {
        Task<LeaseAgreement> AddAsync(LeaseAgreement entity);
        Task<LeaseAgreement> GetByIdAsync(int id);
        Task<List<LeaseAgreement>> GetAllAsync();
        Task<List<LeaseAgreement>> GetByTenantIdAsync(int tenantId);
        Task UpdateAsync(LeaseAgreement entity);
        Task DeleteAsync(int id);
    }
}