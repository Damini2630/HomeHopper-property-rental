using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Services;


//using OnlineRentalPropertyManagement.DTOS;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService; private readonly INotificationService _notificationService; private readonly ApplicationDbContext _context;

        public TenantController(
                ITenantService tenantService,
                INotificationService notificationService,
                ApplicationDbContext context)
        {
            _tenantService = tenantService;
            _notificationService = notificationService;
            _context = context;
        }

        // Register a new tenant
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TenantRegistrationDTO tenantRegistrationDTO)
        {
            var existingTenant = await _tenantService.GetTenantByEmailAsync(tenantRegistrationDTO.Email);
            if (existingTenant != null)
            {
                return BadRequest("A tenant with this email already exists.");
            }

            var tenant = await _tenantService.AddTenantAsync(tenantRegistrationDTO);
            if (tenant != null)
            {
                return Ok($"Tenant registered successfully. The tenant ID is: {tenant.TenantID}");
            }

            return BadRequest("Registration failed.");
        }

        // Tenant login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TenantLoginDTO tenantLoginDTO)
        {
            var token = await _tenantService.LoginTenantAsync(tenantLoginDTO);
            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { Token = token });
        }

        // Get lease agreements for the tenant
        [HttpGet("lease-agreements")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> GetLeaseAgreements()
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !int.TryParse(tenantIdClaim, out var tenantId))
            {
                return BadRequest(new { Success = false, Message = "Invalid tenant ID." });
            }

            var leaseAgreements = await _tenantService.GetLeaseAgreementsAsync(tenantId);
            return Ok(new { Success = true, LeaseAgreements = leaseAgreements });
        }

        // Get maintenance requests for the tenant
        [HttpGet("maintenance-requests")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> GetMaintenanceRequests()
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !int.TryParse(tenantIdClaim, out var tenantId))
            {
                return BadRequest(new { Success = false, Message = "Invalid tenant ID." });
            }

            var maintenanceRequests = await _tenantService.GetMaintenanceRequestsAsync(tenantId);
            return Ok(new { Success = true, MaintenanceRequests = maintenanceRequests });
        }

        // Get late payment notifications
        [HttpGet("late-payment-notifications")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> GetLatePaymentNotifications()
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !int.TryParse(tenantIdClaim, out var tenantId))
            {
                return BadRequest(new { Success = false, Message = "Invalid tenant ID." });
            }

            var notifications = await _tenantService.GetLatePaymentNotificationsAsync(tenantId);
            return Ok(new { Success = true, Notifications = notifications });
        }

        // Check application status
        [HttpGet("application-status/{applicationId}")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> CheckApplicationStatus(int applicationId)
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !int.TryParse(tenantIdClaim, out var tenantId))
            {
                return BadRequest(new { Success = false, Message = "Invalid tenant ID." });
            }

            var applicationStatus = await _tenantService.CheckApplicationStatusAsync(applicationId);
            if (string.IsNullOrEmpty(applicationStatus))
            {
                return NotFound(new { Success = false, Message = "No application found for the provided application ID." });
            }

            return Ok(new { Success = true, Message = applicationStatus });
        }

        // Get all notifications for the tenant
        [HttpGet("profile")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetTenantProfile()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    return BadRequest("Invalid token: Identity not found or not authenticated.");
                }

                var claims = identity.Claims.ToList();

                // Optional: Log all claims for debugging
                foreach (var claim in claims)
                {
                    Console.WriteLine($"CLAIM TYPE: {claim.Type} - CLAIM VALUE: {claim.Value}");
                }

                var tenantIdClaim = claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out var tenantId))
                {
                    return BadRequest("Invalid token: Tenant ID claim missing or not an integer.");
                }

                var tenant = await _context.Tenants.FindAsync(tenantId);
                if (tenant == null)
                    return NotFound("Tenant not found.");

                return Ok(tenant);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Add this to TenantController.cs

        [HttpPut("update-profile")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> UpdateTenantProfile([FromBody] TenantUpdateDTO updatedTenant)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity; if (identity == null) return BadRequest("Invalid token.");

                var tenantIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim, out int tenantId))
                    return BadRequest("Invalid tenant ID.");

                var tenant = await _context.Tenants.FindAsync(tenantId);
                if (tenant == null)
                    return NotFound("Tenant not found.");

                // Update fields
                tenant.Name = updatedTenant.Name;
                tenant.Email = updatedTenant.Email;
                tenant.ContactDetails = updatedTenant.ContactDetails;
                tenant.Password = updatedTenant.Password; // Assume already hashed

                await _context.SaveChangesAsync();

                return Ok("Tenant profile updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }

        }

        [HttpDelete("delete")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> DeleteTenant()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity; if (identity == null) return BadRequest("Invalid token.");

                var tenantIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim, out int tenantId))
                    return BadRequest("Invalid tenant ID.");

                var tenant = await _context.Tenants.FindAsync(tenantId);
                if (tenant == null)
                    return NotFound("Tenant not found.");

                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();

                return Ok("Tenant profile deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }

        }

        [HttpPut("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetTenantPassword([FromBody] ResetPasswordDTO dto)
        {
            var result = await _tenantService.ResetPasswordAsync(dto.Email, dto.NewPassword);
            if (!result)
            {
                return BadRequest("Tenant profile not found.");
            }
            return Ok("Profile updated successfully.");
        }



    }


}