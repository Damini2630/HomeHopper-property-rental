using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalApplicationController : ControllerBase
    {
        private readonly IRentalApplicationService _rentalApplicationService;

        public RentalApplicationController(IRentalApplicationService rentalApplicationService)
        {
            _rentalApplicationService = rentalApplicationService;
        }

        [HttpGet]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllRentalApplications()
        {
            var rentalApplications = await _rentalApplicationService.GetAllRentalApplicationsAsync();
            return Ok(rentalApplications);
        }

        [HttpPost("submit")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> SubmitRentalApplication([FromBody] RentalApplicationDTO rentalApplicationDTO)
        {
            var tenantId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            rentalApplicationDTO.TenantID = tenantId;

            var result = await _rentalApplicationService.SubmitRentalApplicationAsync(rentalApplicationDTO);
            return Ok(new { Success = true, Message = "Rental application submitted successfully." });
        }

        [HttpGet("property/{propertyId}")]
        [Authorize(Roles = "owner")]
        public async Task<ActionResult<List<RentalApplication>>> GetRentalApplicationsByPropertyId(int propertyId)
        {
            var applications = await _rentalApplicationService.GetRentalApplicationsByPropertyIdAsync(propertyId);
            return Ok(applications);
        }


        [HttpGet("owner")]
        [Authorize(Roles = "owner")]
        public async Task<ActionResult<List<RentalApplicationDTO>>> GetRentalApplicationsForCurrentOwner()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (ownerIdClaim == null || !int.TryParse(ownerIdClaim.Value, out int ownerId))
            {
                return Unauthorized(new { Message = "Owner ID not found in token." });
            }

            var applications = await _rentalApplicationService.GetRentalApplicationsByOwnerIdAsync(ownerId);
            return Ok(applications);
        }



        [HttpGet("tenant")]
        [Authorize(Roles = "tenant")]
        public async Task<ActionResult<List<RentalApplication>>> GetRentalApplicationsForCurrentTenant()
        {
            var tenantId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var applications = await _rentalApplicationService.GetRentalApplicationsByTenantIdAsync(tenantId);
            return Ok(applications);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "tenant,owner")]
        public async Task<ActionResult<RentalApplication>> GetRentalApplicationById(int id)
        {
            var application = await _rentalApplicationService.GetRentalApplicationByIdAsync(id);
            return application != null ? Ok(application) : NotFound();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> UpdateRentalApplication(int id, [FromBody] RentalApplicationDTO updatedApplicationDTO)
        {
            var tenantId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var existingApplication = await _rentalApplicationService.GetRentalApplicationByIdAsync(id);

            if (existingApplication == null)
            {
                return NotFound();
            }

            if (existingApplication.TenantID != tenantId)
            {
                return Unauthorized(new { Message = "You are not authorized to update this application." });
            }

            if (existingApplication.Status.ToLower() != "pending")
            {
                return BadRequest(new { Message = "You can only update applications with 'Pending' status." });
            }

            updatedApplicationDTO.RentalApplicationID = id; // Ensure the ID is set for updating
            var result = await _rentalApplicationService.UpdateRentalApplicationAsync(updatedApplicationDTO);

            return result ? Ok(new { Message = "Rental application updated successfully." }) : StatusCode(500, new { Message = "Failed to update rental application." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "tenant,admin")]
        public async Task<IActionResult> DeleteRentalApplication(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);
            var isTenant = userRoleClaim?.Value.ToLower() == "tenant";

            var existingApplication = await _rentalApplicationService.GetRentalApplicationByIdAsync(id);

            if (existingApplication == null)
            {
                return NotFound();
            }

            if (isTenant && existingApplication.TenantID != userId)
            {
                return Unauthorized(new { Message = "You are not authorized to delete this application." });
            }

            var result = await _rentalApplicationService.DeleteRentalApplicationAsync(id);

            return result ? Ok(new { Message = "Rental application deleted successfully." }) : NotFound();
        }

        [HttpPut("status/{id}")]
        //[Authorize(Roles = "owner,admin")]
        public async Task<IActionResult> UpdateRentalApplicationStatus(int id, [FromBody] string status)
        {
            var result = await _rentalApplicationService.UpdateRentalApplicationStatusAsync(id, status);
            return result ? Ok() : NotFound();
        }
    }
}