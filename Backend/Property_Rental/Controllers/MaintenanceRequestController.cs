using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineRentalPropertyManagement.Services.Interfaces;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace OnlineRentalPropertyManagement.Controllers
{
    [ApiController]
    [Route("api/maintenance")]
    public class MaintenanceRequestController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ApplicationDbContext _context;

        public MaintenanceRequestController(IMaintenanceService maintenanceService, ApplicationDbContext context)
        {
            _maintenanceService = maintenanceService;
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "tenant")]
        public async Task<ActionResult<MaintenanceRequest>> CreateRequest([FromBody] MaintenanceRequestDTO request)
        {
            if (request.TenantID == 0 || request.PropertyID == 0 || string.IsNullOrEmpty(request.IssueDescription))
            {
                return BadRequest("Missing required fields.");
            }

            var maintenanceRequest = new MaintenanceRequest
            {
                PropertyID = request.PropertyID,
                TenantID = request.TenantID,
                OwnerID = request.OwnerID,
                IssueDescription = request.IssueDescription,
                Status = request.Status,
                AssignedDate = request.AssignedDate
            };

            _context.MaintenanceRequest.Add(maintenanceRequest);
            await _context.SaveChangesAsync();

            var notification = new MaintenanceNotification
            {
                tenantid = request.OwnerID, // Correctly assign OwnerID for the notification
                Message = $"New maintenance request from Tenant {request.TenantID} for Property {request.PropertyID}.",
                CreatedAt = DateTime.UtcNow
            };

            await _maintenanceService.AddOwnerNotificationAsync(notification);

            return Ok(maintenanceRequest);
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDTO>>> GetMaintenanceRequests()
        {
            var requests = await _maintenanceService.GetAllMaintenanceRequestsAsync();
            if (requests == null || !requests.Any())
            {
                return Ok(new { Message = "No maintenance requests found." });
            }
            var requestDTOs = requests.Select(r => new MaintenanceRequestDTO
            {
                RequestID = r.RequestID,
                PropertyID = r.PropertyID,
                TenantID = r.TenantID,
                OwnerID = r.OwnerID,
                IssueDescription = r.IssueDescription,
                Status = r.Status,
                AssignedDate = r.AssignedDate
            });
            return Ok(new { Message = "Maintenance requests retrieved successfully", Requests = requestDTOs });
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "owner,tenant")]
        public async Task<ActionResult<MaintenanceRequestDTO>> GetMaintenanceRequest(int id)
        {
            var request = await _maintenanceService.GetMaintenanceRequestByIdAsync(id);
            return request != null
                ? Ok(new { Message = "Maintenance request retrieved successfully", Request = request })
                : NotFound(new { Message = "Maintenance request not found" });
        }

        [HttpPost("submit")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> SubmitServiceRequest([FromBody] ServiceRequestDTO dto)
        {
            // Validation: ensure the request exists
            var request = await _context.MaintenanceRequest.FindAsync(dto.RequestID);
            if (request == null)
            {
                return BadRequest(new { Message = "Maintenance request not found. Cannot create service request." });
            }

            if (string.IsNullOrWhiteSpace(dto.AgentName) ||
                string.IsNullOrWhiteSpace(dto.AgentContactNo) ||
                string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new { Message = "All required fields must be filled." });
            }

            var serviceRequest = new Servicerequest
            {
                RequestID = dto.RequestID,
                AgentName = dto.AgentName,
                AgentContactNo = dto.AgentContactNo,
                Status = dto.Status,
                ServiceBill = dto.ServiceBill
            };

            _context.Servicerequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Service request submitted successfully." });
        }

        [HttpGet("service-bill/{RequestID}")]
        [Authorize(Roles = "owner,tenant,admin")]
        public async Task<IActionResult> GetServiceBill(int RequestID)
        {
            var serviceRequest = await _context.Servicerequests.FindAsync(RequestID);
            if (serviceRequest == null)
            {
                return NotFound(new { Message = "Service request not found" });
            }
            return Ok(new { Message = "Service bill retrieved successfully", ServiceBill = serviceRequest.ServiceBill });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> DeleteMaintenanceRequest(int id)
        {
            var success = await _maintenanceService.DeleteMaintenanceRequestAsync(id);
            return success ? NoContent() : NotFound(new { Message = "Maintenance request not found" });
        }

        [HttpGet("by-tenant/{tenantId}")]
        //[Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetRequestsByTenant(int tenantId)
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (tenantIdClaim != tenantId.ToString())
            {
                return Unauthorized("Unauthorized tenant access.");
            }

            var requests = await _maintenanceService.GetRequestsByTenantIdAsync(tenantId);
            return Ok(requests);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] MaintenanceRequestDTO dto)
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (tenantIdClaim != dto.TenantID.ToString())
            {
                return Unauthorized("Unauthorized tenant update.");
            }

            var success = await _maintenanceService.UpdateMaintenanceRequestAsync(id, dto);
            if (success)
            {
                var notification = new MaintenanceNotification
                {
                    tenantid = dto.OwnerID,
                    Message = $"Maintenance request #{id} updated by Tenant {dto.TenantID}.",
                    CreatedAt = DateTime.UtcNow
                };
                await _maintenanceService.AddOwnerNotificationAsync(notification);
            }

            return success ? Ok(new { Message = "Updated successfully" }) : NotFound(new { Message = "Not found" });
        }
        [HttpPut("status")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> UpdateStatus([FromBody] MaintenanceStatusDTO dto)
        {
            try
            {
                Console.WriteLine($"Updating status for RequestID: {dto.RequestID}, Status: {dto.Status}");
                var success = await _maintenanceService.UpdateRequestStatus(dto.RequestID, dto.Status);
                if (success)
                {
                    return Ok(new { Message = "Status updated successfully" });
                }
                else
                {
                    Console.WriteLine("Request not found.");
                    return BadRequest(new { Message = "Request not found" });
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                return StatusCode(500, new { Message = "An error occurred while updating the status." });
            }
        }



        [HttpGet("owner-notifications/{ownerId}")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetOwnerNotifications(int ownerId)
        {
            var notifications = await _maintenanceService.GetOwnerNotificationsAsync(ownerId);
            return Ok(notifications);
        }

        [HttpPut("read-notification/{notificationId}")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            var success = await _maintenanceService.MarkNotificationAsReadAsync(notificationId);
            return success ? Ok(new { Message = "Marked as read" }) : NotFound(new { Message = "Notification not found" });
        }
    }
}
        