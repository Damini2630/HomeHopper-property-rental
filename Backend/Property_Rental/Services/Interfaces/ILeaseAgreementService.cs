using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface ILeaseAgreementService
    {
        Task<LeaseAgreement> CreateLeaseAgreementAsync(LeaseAgreement leaseAgreement);
        Task AddOwnerDocumentsAsync(int leaseID, string ownerSignaturePath, string ownerDocumentPath);
        Task<List<LeaseAgreement>> GetLeaseAgreementsByTenantIdAsync(int tenantId);
        Task<LeaseAgreement> GetLeaseAgreementByIdAsync(int leaseId);
        Task<string> GenerateLegalDocumentAsync(int leaseID);
        Task<LeaseAgreement> AcceptLeaseAgreementAsync(int leaseID);
        Task<LeaseAgreement> RejectLeaseAgreementAsync(int leaseID);
        Task DeleteLeaseAgreementAsync(int leaseId);
        Task<bool> CheckIfLeaseAgreementExistsForPropertyAndTenantAsync(int propertyId, int tenantId);
    }
}
