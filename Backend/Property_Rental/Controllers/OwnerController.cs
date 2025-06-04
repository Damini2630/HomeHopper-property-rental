using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Services;
using OnlineRentalPropertyManagement.Services.Interfaces; // Corrected using directive
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        private readonly IRentalApplicationService _rentalApplicationService; // Changed to interface
        private readonly ILogger<OwnerController> _logger;
        private readonly INotificationService _notificationService;

        public OwnerController(
            IOwnerService ownerService,
            IRentalApplicationService rentalApplicationService, // Using the interface
            ILogger<OwnerController> logger,
            INotificationService notificationService)
        {
            _ownerService = ownerService;
            _notificationService = notificationService;
            _rentalApplicationService = rentalApplicationService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] OwnerRegistrationDTO ownerRegistrationDTO)
        {
            var existingOwner = await _ownerService.GetOwnerByEmailAsync(ownerRegistrationDTO.Email);
            if (existingOwner != null)
            {
                return BadRequest("An owner with this email already exists.");
            }

            var owner = await _ownerService.AddOwnerAsync(ownerRegistrationDTO);
            if (owner != null)
            {
                return Ok($"Owner registered successfully. Your owner ID is: {owner.OwnerID}");
            }
            return BadRequest("Registration failed.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OwnerLoginDTO ownerLoginDTO)
        {
            var token = await _ownerService.LoginOwnerAsync(ownerLoginDTO);
            if (token == null)
            {
                return Unauthorized(new { Success = false, Message = "Invalid email or password." });
            }
            return Ok(new { Success = true, Token = token });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOwnerById()
        {
            try
            {
                var ownerIdClaim = User.FindFirst("ownerId");
                if (ownerIdClaim == null)
                {
                    _logger.LogError("Owner ID claim not found in JWT token.");
                    return Unauthorized("Owner ID claim not found.");
                }

                if (!int.TryParse(ownerIdClaim.Value, out int ownerId))
                {
                    _logger.LogError("Invalid owner ID format in JWT token.");
                    return BadRequest("Invalid owner ID format.");
                }

                var owner = await _ownerService.GetOwnerByIdAsync(ownerId);
                if (owner == null)
                {
                    _logger.LogWarning($"Owner with ID {ownerId} not found.");
                    return NotFound();
                }

                return Ok(owner);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the owner by ID.");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("lease-agreements")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetLeaseAgreements()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
            {
                return BadRequest(new { Success = false, Message = "Invalid owner ID." });
            }

            var leaseAgreements = await _ownerService.GetLeaseAgreementsAsync(ownerId);
            if (leaseAgreements == null)
            {
                return NotFound(new { Success = false, Message = "No lease agreements found." });
            }

            return Ok(new { Success = true, LeaseAgreements = leaseAgreements });
        }

        [HttpGet("maintenance-requests")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetMaintenanceRequests()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
            {
                return BadRequest(new { Success = false, Message = "Invalid owner ID." });
            }

            var maintenanceRequests = await _ownerService.GetMaintenanceRequestsAsync(ownerId);
            return Ok(new { Success = true, MaintenanceRequests = maintenanceRequests });
        }

        [HttpGet("late-payment-notifications")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetLatePaymentNotifications()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
            {
                return BadRequest(new { Success = false, Message = "Invalid owner ID." });
            }

            var notifications = await _ownerService.GetLatePaymentNotificationsAsync(ownerId);
            return Ok(new { Success = true, Notifications = notifications });
        }

        [HttpGet("rental-application-notifications")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetRentalApplicationNotifications()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
            {
                return BadRequest(new { Success = false, Message = "Invalid owner ID." });
            }

            var notifications = await _notificationService.GetNotificationsByOwnerIdAsync(ownerId);
            return Ok(new { Success = true, Notifications = notifications });
        }
        [HttpPut("update")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> UpdateOwner([FromBody] OwnerUpdateDTO ownerUpdateDTO)
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
            {
                return BadRequest("Invalid owner ID.");
            }

            var result = await _ownerService.UpdateOwnerAsync(ownerId, ownerUpdateDTO);
            if (result)
            {
                return Ok("Owner updated successfully.");
            }
            return BadRequest("Failed to update owner.");
        }


        [HttpDelete("delete")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> DeleteOwnerAsync()
        {
            try
            {
                var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
                {
                    _logger.LogWarning("Invalid owner ID claim: {OwnerIdClaim}", ownerIdClaim);
                    return BadRequest("Invalid owner ID.");
                }

                var result = await _ownerService.DeleteOwnerAsync(ownerId);
                if (result)
                {
                    return Ok("Owner deleted successfully.");
                }
                return BadRequest("Failed to delete owner.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the owner.");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("notifications")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !int.TryParse(ownerIdClaim, out var ownerId))
            {
                return BadRequest(new { Success = false, Message = "Invalid owner ID." });
            }

            var notifications = await _notificationService.GetNotificationsByOwnerIdAsync(ownerId);
            return Ok(new { Success = true, Notifications = notifications });
        }
        [HttpPut("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDTO dto)
        {
            var success = await _ownerService.ResetPasswordAsync(dto.Email, dto.NewPassword);

            if (success)
                return Ok("Password reset successfully.");

            return BadRequest("No owner found with that email.");
        }
    }
}