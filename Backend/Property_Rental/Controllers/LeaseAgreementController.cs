using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaseAgreementController : ControllerBase
    {
        private readonly ILeaseAgreementService _leaseAgreementService;
        private readonly ApplicationDbContext _context; // Consider using repository for direct queries

        public LeaseAgreementController(
            ILeaseAgreementService leaseAgreementService,
            ApplicationDbContext context)
        {
            _leaseAgreementService = leaseAgreementService;
            _context = context;
        }

        // Tenant Post: Create a new lease agreement
        [HttpPost]
        [Authorize(Roles = "tenant")]

        public async Task<IActionResult> CreateLeaseAgreement([FromBody] LeaseAgreementDTO leaseAgreementDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantIdFromToken))
                {
                    return Unauthorized("Invalid Tenant ID in the token.");
                }

                if (leaseAgreementDTO.TenantID != tenantIdFromToken)
                {
                    return Forbid("You are not authorized to create a lease agreement for this Tenant ID.");
                }

                var leaseAgreement = new LeaseAgreement
                {
                    PropertyID = leaseAgreementDTO.PropertyID,
                    TenantID = tenantIdFromToken,
                    StartDate = leaseAgreementDTO.StartDate,
                    EndDate = leaseAgreementDTO.EndDate,
                    TenantSignaturePath = leaseAgreementDTO.TenantSignaturePath,
                    TenantDocumentPath = leaseAgreementDTO.TenantDocumentPath,
                    Status = "Pending"
                };

                var createdLease = await _leaseAgreementService.CreateLeaseAgreementAsync(leaseAgreement);
                return Ok(createdLease);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Owner Put: Add owner documents (signature and document paths)
        [HttpPut("{leaseID}/owner/documents")]
        [Authorize(Roles = "owner")]

        public async Task<IActionResult> AddOwnerDocuments(int leaseID, [FromBody] OwnerDocumentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var leaseAgreement = await _context.LeaseAgreements
                    .Include(la => la.Property)
                    .FirstOrDefaultAsync(la => la.LeaseID == leaseID);

                if (leaseAgreement == null)
                {
                    return NotFound("Lease agreement not found.");
                }

                if (leaseAgreement.Property.OwnerID.ToString() != ownerId)
                {
                    return Forbid("You are not authorized to update documents for this lease agreement.");
                }

                await _leaseAgreementService.AddOwnerDocumentsAsync(
                    leaseID,
                    request.OwnerSignaturePath,
                    request.OwnerDocumentPath
                );

                var updatedLeaseStatus = await _context.LeaseAgreements.FindAsync(leaseID);
                return Ok(updatedLeaseStatus); // Return updated lease status after document upload
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Owner Put: Accept or Reject the lease agreement
        [HttpPut("{leaseID}/owner/status")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> UpdateLeaseStatus(int leaseID, [FromBody] LeaseStatusUpdateRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var leaseAgreement = await _context.LeaseAgreements
                    .Include(la => la.Property)
                    .FirstOrDefaultAsync(la => la.LeaseID == leaseID);

                if (leaseAgreement == null)
                {
                    return NotFound("Lease agreement not found.");
                }

                if (leaseAgreement.Property.OwnerID.ToString() != ownerId)
                {
                    return Forbid("You are not authorized to update the status of this lease agreement.");
                }

                LeaseAgreement updatedLease;
                if (request.Action.ToLower() == "accept")
                {
                    updatedLease = await _leaseAgreementService.AcceptLeaseAgreementAsync(leaseID);
                    return Ok(updatedLease);
                }
                else if (request.Action.ToLower() == "reject")
                {
                    updatedLease = await _leaseAgreementService.RejectLeaseAgreementAsync(leaseID);
                    return Ok(updatedLease);
                }
                else
                {
                    return BadRequest("Invalid action. Use 'accept' or 'reject'.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Tenant Get: Retrieve all leases for the authenticated tenant
        [HttpGet("tenant/leases")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetMyLeases()
        {
            try
            {
                var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantId))
                {
                    return Unauthorized("Invalid Tenant ID in the token.");
                }

                var leases = await _context.LeaseAgreements
                    .Where(la => la.TenantID == tenantId)
                    .Include(la => la.Property)
                    .Include(la => la.Tenant)
                    .Include(la => la.OwnerDocument)
                    .OrderByDescending(la => la.StartDate)
                    .ToListAsync();
                // Map 'leases' (List<LeaseAgreement>) to List<LeaseAgreementResponseDTO> here
                return Ok(leases); // For now, returning the model directly
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Get: Retrieve specific lease agreement details (accessible to both tenant and owner involved)
        [HttpGet("{leaseID}")]
        [Authorize(Roles = "tenant,owner")]
        public async Task<IActionResult> GetLeaseAgreementDetails(int leaseID)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                var leaseAgreement = await _context.LeaseAgreements
                    .Include(la => la.Property)
                        .ThenInclude(p => p.Owner)
                    .Include(la => la.Tenant)
                    .Include(la => la.OwnerDocument)
                    .FirstOrDefaultAsync(la => la.LeaseID == leaseID);

                if (leaseAgreement == null)
                {
                    return NotFound("Lease agreement not found.");
                }

                // Authorization check
                if (userRole == "tenant" && leaseAgreement.TenantID.ToString() != userId)
                {
                    return Forbid("You are not authorized to view this lease agreement.");
                }
                if (userRole == "owner" && leaseAgreement.Property.OwnerID.ToString() != userId)
                {
                    return Forbid("You are not authorized to view this lease agreement.");
                }

                // Map 'leaseAgreement' (LeaseAgreement) to LeaseAgreementResponseDTO here
                return Ok(leaseAgreement); // For now, returning the model directly
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{leaseID}/document")]
        [Authorize(Roles = "tenant,owner")]
        public async Task<IActionResult> GetLegalDocument(int leaseID)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                var leaseAgreement = await _context.LeaseAgreements
                    .Include(la => la.Property)
                        .ThenInclude(p => p.Owner)
                    .Include(la => la.Tenant)
                    .FirstOrDefaultAsync(la => la.LeaseID == leaseID);

                if (leaseAgreement == null)
                {
                    return NotFound("Lease agreement not found.");
                }

                // Authorization check
                if (userRole == "tenant" && leaseAgreement.TenantID.ToString() != userId)
                {
                    return Forbid("You are not authorized to view this document.");
                }
                if (userRole == "owner" && leaseAgreement.Property.OwnerID.ToString() != userId)
                {
                    return Forbid("You are not authorized to view this document.");
                }

                var documentPath = await _leaseAgreementService.GenerateLegalDocumentAsync(leaseID);
                var documentContent = await System.IO.File.ReadAllTextAsync(documentPath);

                return Content(documentContent, "text/plain");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{leaseId}")]
        [Authorize(Roles = "tenant")] // Assuming only tenants can delete their pending agreements
        public async Task<IActionResult> DeleteLeaseAgreement(int leaseId)
        {
            try
            {
                var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantIdFromToken))
                {
                    return Unauthorized("Invalid Tenant ID in the token.");
                }

                var existingLease = await _context.LeaseAgreements // Consider using service to fetch
                    .FirstOrDefaultAsync(la => la.LeaseID == leaseId);

                if (existingLease == null)
                {
                    return NotFound("Lease agreement not found.");
                }

                if (existingLease.TenantID != tenantIdFromToken)
                {
                    return Forbid("You are not authorized to delete this lease agreement.");
                }

                if (existingLease.Status.ToLower() == "accepted")
                {
                    return BadRequest("Cannot delete an accepted lease agreement.");
                }

                await _leaseAgreementService.DeleteLeaseAgreementAsync(leaseId);
                return NoContent(); // Successful deletion, no content to return
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("property/{propertyId}/tenant/lease-exists")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> CheckIfLeaseAgreementExistsForProperty(int propertyId)
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                return Unauthorized();
            }

            var exists = await _leaseAgreementService.CheckIfLeaseAgreementExistsForPropertyAndTenantAsync(propertyId, tenantId);
            return Ok(new { Exists = exists });
        }
    }

    // DTO for updating lease status
   
}