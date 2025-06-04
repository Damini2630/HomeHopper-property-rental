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
    public class LeaseAgreementService : ILeaseAgreementService
    {
        private readonly ILeaseAgreementRepository _leaseAgreementRepository;
        private readonly IOwnerDocumentRepository _ownerDocumentRepository;
        private readonly ApplicationDbContext _context; // Consider injecting only repositories

        public LeaseAgreementService(ILeaseAgreementRepository leaseAgreementRepository, IOwnerDocumentRepository ownerDocumentRepository, ApplicationDbContext context)
        {
            _leaseAgreementRepository = leaseAgreementRepository;
            _ownerDocumentRepository = ownerDocumentRepository;
            _context = context;
        }

        public async Task<LeaseAgreement> CreateLeaseAgreementAsync(LeaseAgreement leaseAgreement)
        {
            return await _leaseAgreementRepository.AddAsync(leaseAgreement);
        }
        public async Task DeleteLeaseAgreementAsync(int leaseId)
        {
            var leaseAgreement = await _leaseAgreementRepository.GetByIdAsync(leaseId);
            if (leaseAgreement == null)
            {
                throw new Exception($"Lease agreement with ID {leaseId} not found.");
            }

            if (leaseAgreement.Status.ToLower() == "accepted")
            {
                throw new Exception($"Cannot delete lease agreement with ID {leaseId} as its status is 'Accepted'.");
            }

            await _leaseAgreementRepository.DeleteAsync(leaseId);
        }

        public async Task<bool> CheckIfLeaseAgreementExistsForPropertyAndTenantAsync(int propertyId, int tenantId)
        {
            return await _context.LeaseAgreements
                .AnyAsync(la => la.PropertyID == propertyId && la.TenantID == tenantId);
        }
        public async Task AddOwnerDocumentsAsync(int leaseID, string ownerSignaturePath, string ownerDocumentPath)
        {
            var leaseAgreement = await _leaseAgreementRepository.GetByIdAsync(leaseID);
            if (leaseAgreement == null)
            {
                throw new Exception("Lease agreement not found.");
            }

            var ownerDocument = await _ownerDocumentRepository.GetByLeaseIdAsync(leaseID);
            if (ownerDocument == null)
            {
                ownerDocument = new OwnerDocument { LeaseID = leaseID };
                await _ownerDocumentRepository.AddAsync(ownerDocument);
            }

            ownerDocument.OwnerSignaturePath = ownerSignaturePath;
            ownerDocument.OwnerDocumentPath = ownerDocumentPath;
            await _ownerDocumentRepository.UpdateAsync(ownerDocument);
        }

        public async Task<List<LeaseAgreement>> GetLeaseAgreementsByTenantIdAsync(int tenantId)
        {
            return await _leaseAgreementRepository.GetByTenantIdAsync(tenantId);
        }

        public async Task<LeaseAgreement> GetLeaseAgreementByIdAsync(int leaseId)
        {
            return await _leaseAgreementRepository.GetByIdAsync(leaseId);
        }

        public async Task<string> GenerateLegalDocumentAsync(int leaseID)
        {
            var lease = await _leaseAgreementRepository.GetByIdAsync(leaseID);
            if (lease == null)
            {
                throw new Exception("Lease agreement not found.");
            }
            return $"./legal_documents/lease_{leaseID}.txt";
        }

        public async Task<LeaseAgreement> AcceptLeaseAgreementAsync(int leaseID)
        {
            var leaseAgreement = await _leaseAgreementRepository.GetByIdAsync(leaseID);
            if (leaseAgreement == null)
            {
                throw new Exception("Lease agreement not found.");
            }

            //var ownerDocument = await _ownerDocumentRepository.GetByLeaseIdAsync(leaseID);
            //if (ownerDocument?.OwnerSignaturePath == null || ownerDocument?.OwnerDocumentPath == null)
            //{
            //    throw new Exception("Owner must upload their documents before accepting the lease.");
            //}

            if (leaseAgreement.Status == "Pending")
            {
                leaseAgreement.Status = "Accepted"; // Updated status on acceptance
                await _leaseAgreementRepository.UpdateAsync(leaseAgreement);
                return leaseAgreement;
            }
            else
            {
                throw new Exception($"Lease agreement is not in a state where it can be accepted. Current status: {leaseAgreement.Status}");
            }
        }

        public async Task<LeaseAgreement> RejectLeaseAgreementAsync(int leaseID)
        {
            var leaseAgreement = await _leaseAgreementRepository.GetByIdAsync(leaseID);
            if (leaseAgreement == null)
            {
                throw new Exception("Lease agreement not found.");
            }

            if (leaseAgreement.Status == "Pending")
            {
                leaseAgreement.Status = "Rejected"; // Status on rejection
                await _leaseAgreementRepository.UpdateAsync(leaseAgreement);
                return leaseAgreement;
            }
            else
            {
                throw new Exception($"Lease agreement is not in a state where it can be rejected. Current status: {leaseAgreement.Status}");
            }
        }
    }
}