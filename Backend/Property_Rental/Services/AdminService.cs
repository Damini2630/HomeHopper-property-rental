
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System.Security.Claims;

namespace OnlineRentalPropertyManagement.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ITokenService tokenService, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<string> LoginAdminAsync(AdminLoginDTO adminLoginDTO)
        {
            var admin = await _adminRepository.GetAdminByEmailAsync(adminLoginDTO.Email);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(adminLoginDTO.Password, admin.Password))
            {
                _logger.LogWarning("Login failed for email: {Email}", adminLoginDTO.Email);
                return null; // Invalid credentials
            }

            _logger.LogInformation("Admin logged in successfully: {Email}", adminLoginDTO.Email);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, admin.AdminID.ToString()), // Use NameIdentifier for admin ID
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, "admin")
        };

            return _tokenService.GenerateToken(claims);
        }
    }
}
