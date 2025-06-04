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
using OnlineRentalPropertyManagement.Repositories;

namespace OnlineRentalPropertyManagement.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ILogger<TenantService> _logger;

        public TenantService(
            ITenantRepository tenantRepository,
            INotificationService notificationService,
            IConfiguration configuration,
            ITokenService tokenService,
            ILogger<TenantService> logger)
        {
            _tenantRepository = tenantRepository;
            _notificationService = notificationService;
            _configuration = configuration;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Tenant> AddTenantAsync(TenantRegistrationDTO tenantRegistrationDTO)
        {
            var existingTenant = await _tenantRepository.GetTenantByEmailAsync(tenantRegistrationDTO.Email);
            if (existingTenant != null)
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", tenantRegistrationDTO.Email);
                return null; // Email already exists
            }

            var tenant = new Tenant
            {
                Name = tenantRegistrationDTO.Name,
                Email = tenantRegistrationDTO.Email,
                Password = tenantRegistrationDTO.Password, // Storing plain text password
                ContactDetails = tenantRegistrationDTO.ContactDetails
            };

            var result = await _tenantRepository.AddTenantAsync(tenant);
            if (result)
            {
                return tenant;
            }
            return null;
        }

        public async Task<string> LoginTenantAsync(TenantLoginDTO tenantLoginDTO)
        {
            var tenant = await _tenantRepository.GetTenantByEmailAsync(tenantLoginDTO.Email);

            if (tenant == null || tenant.Password != tenantLoginDTO.Password) // Plain text comparison
            {
                _logger.LogWarning("Login failed for email: {Email}", tenantLoginDTO.Email);
                return null; // Invalid credentials
            }

            _logger.LogInformation("Tenant logged in successfully: {Email}", tenantLoginDTO.Email);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, tenant.TenantID.ToString()),
                new Claim(ClaimTypes.Email, tenant.Email),
                new Claim(ClaimTypes.Role, "tenant")
            };

            return _tokenService.GenerateToken(claims);
        }

        public async Task<Tenant> GetTenantByEmailAsync(string email)
        {
            return await _tenantRepository.GetTenantByEmailAsync(email);
        }

        public async Task<List<LeaseAgreement>> GetLeaseAgreementsAsync(int tenantId)
        {
            return await _tenantRepository.GetLeaseAgreementsByTenantIdAsync(tenantId);
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync(int tenantId)
        {
            return await _tenantRepository.GetMaintenanceRequestsByTenantIdAsync(tenantId);
        }

        public async Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int tenantId)
        {
            return await _tenantRepository.GetLatePaymentNotificationsAsync(tenantId);
        }

        public async Task<string> CheckApplicationStatusAsync(int applicationId)
        {
            var rentalStatus = await _tenantRepository.CheckApplicationStatusAsync(applicationId);
            var application = rentalStatus.FirstOrDefault(rs => rs.RentalApplicationID == applicationId);

            if (application == null)
            {
                return $"No application found for ID {applicationId}.";
            }

            var isApproved = application.Status == "Approved";
            return isApproved
                ? $"Your rental application for property ID {application.Property.PropertyID} has been approved."
                : $"Your rental application for property ID {application.Property.PropertyID} has been rejected.";
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            return await _tenantRepository.ResetPasswordAsync(email, newPassword);
        }



    }
}