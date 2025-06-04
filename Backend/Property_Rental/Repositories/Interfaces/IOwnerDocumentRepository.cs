using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface IOwnerDocumentRepository
    {

        Task<OwnerDocument> GetByLeaseIdAsync(int leaseId);
        Task<OwnerDocument> AddAsync(OwnerDocument entity);
        Task UpdateAsync(OwnerDocument entity);
    }
}
