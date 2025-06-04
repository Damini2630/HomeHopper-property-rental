using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using OnlineRentalPropertyManagement.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.Services;
using Microsoft.Data.SqlClient;
using Dapper;

namespace OnlineRentalPropertyManagement.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IRentalApplicationService _applicationService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ILogger<OwnerService> _logger;

        public OwnerService(
            IOwnerRepository ownerRepository,
            INotificationService notificationService,
            IRentalApplicationService applicationService,
            IConfiguration configuration,
            ITokenService tokenService,
            ILogger<OwnerService> logger)
        {
            _ownerRepository = ownerRepository;
            _notificationService = notificationService;
            _applicationService = applicationService;
            _configuration = configuration;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Owner> AddOwnerAsync(OwnerRegistrationDTO dto)
        {
            if (await _ownerRepository.GetOwnerByEmailAsync(dto.Email) != null)
                return null;

            var owner = new Owner
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,  // Stored as plain text
                ContactDetails = dto.ContactDetails
            };

            return await _ownerRepository.AddOwnerAsync(owner) ? owner : null;
        }

        public async Task<bool> UpdateOwnerAsync(int ownerId, OwnerUpdateDTO ownerUpdateDTO)
        {
            var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            if (owner == null)
            {
                return false;
            }

            // Update owner properties except email
            owner.Name = ownerUpdateDTO.Name;
            owner.ContactDetails = ownerUpdateDTO.ContactDetails;
            owner.Password = ownerUpdateDTO.Password;
            // Add other properties as needed

            return await _ownerRepository.UpdateOwnerAsync(owner);
        }

        public async Task<bool> DeleteOwnerAsync(int ownerId)
        {
            return await _ownerRepository.DeleteOwnerAsync(ownerId);
        }

        public async Task<string> LoginOwnerAsync(OwnerLoginDTO dto)
        {
            var owner = await _ownerRepository.GetOwnerByEmailAsync(dto.Email);
            if (owner == null || owner.Password != dto.Password)  // Plain text comparison
                return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, owner.OwnerID.ToString()),
                new Claim(ClaimTypes.Email, owner.Email),
                new Claim(ClaimTypes.Role, "owner"),
                new Claim("ownerId", owner.OwnerID.ToString()) // Add this line
            };

            return _tokenService.GenerateToken(claims);
        }

        //public async Task<bool> ResetPasswordAsync(int ownerId, string oldemail, string newPassword)
        //{
        //    var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
        //    if (owner == null || owner.Email != oldemail)  // Plain text check
        //        return false;

        //    owner.Password = newPassword;  // Update to new plain text password
        //    return await _ownerRepository.UpdateOwnerAsync(owner);
        //}

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            return await _ownerRepository.ResetPasswordAsync(email, newPassword);
        }

        public async Task<Owner> GetOwnerByIdAsync(int ownerId)
        {
            return await _ownerRepository.GetOwnerByIdAsync(ownerId);
        }

        public async Task<Owner> GetOwnerByEmailAsync(string email)
        {
            return await _ownerRepository.GetOwnerByEmailAsync(email);
        }
        public async Task<List<LeaseAgreement>> GetLeaseAgreementsAsync(int ownerId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                SELECT 
                    la.LeaseID,
                    la.PropertyID,
                    la.TenantID,
                    la.StartDate,
                    la.EndDate,
                    la.TenantSignaturePath,
                    la.TenantDocumentPath,
                    la.Status
                FROM 
                    LeaseAgreements la
                JOIN 
                    Properties p ON la.PropertyID = p.PropertyID
                JOIN 
                    Owners o ON p.OwnerID = o.OwnerID
                WHERE 
                    o.OwnerID = @OwnerID";

                var parameters = new { OwnerID = ownerId };
                var leaseAgreements = await connection.QueryAsync<LeaseAgreement>(query, parameters);
                return leaseAgreements.ToList();
            }
        }


        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync(int ownerId)
        {
            return await _ownerRepository.GetMaintenanceRequestsByOwnerIdAsync(ownerId);
        }

        public async Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int ownerId)
        {
            return await _ownerRepository.GetLatePaymentNotificationsAsync(ownerId);
        }


        public async Task<bool> UpdateOwnerAsync(OwnerUpdateDTO dto)
        {
            // You'll need logic here to map OwnerUpdateDTO to Owner model
            var owner = await _ownerRepository.GetOwnerByIdAsync(dto.OwnerId); // assuming this exists
            if (owner == null) return false;

            // Update fields (example)
            owner.Name = dto.Name;
            owner.Email = dto.Email;
            owner.ContactDetails = dto.ContactDetails;

            return await _ownerRepository.UpdateOwnerAsync(owner);
        }

    }
}