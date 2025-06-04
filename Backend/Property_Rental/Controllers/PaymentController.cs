using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            ApplicationDbContext context,
            IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        private int? GetCurrentTenantIdFromToken()
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(tenantIdClaim, out var id) ? id : null;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        // Tenant creates payment
        [HttpPost]
        [Authorize(Roles = "tenant")] // Ensure only authenticated tenants can access this
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto paymentDto)
        {
            var tenantId = GetCurrentTenantIdFromToken();
            if (!tenantId.HasValue) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lease = await _context.LeaseAgreements
                .Where(l => l.LeaseID == paymentDto.LeaseID && l.TenantID == tenantId.Value)
                .Include(l => l.Property)
                .FirstOrDefaultAsync();

            if (lease == null)
                return NotFound("Lease agreement not found or does not belong to the current tenant.");

            if (!await _paymentService.ValidatePaymentAmountAsync(paymentDto.LeaseID, paymentDto.Amount))
                return BadRequest("Payment amount doesn't match the rent for this lease.");

            var payment = new Payment
            {
                LeaseID = paymentDto.LeaseID,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod
            };

            await _paymentService.AddPaymentAsync(payment);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { id = payment.PaymentID },
                new PaymentHistoryDto
                {
                    InvoiceNumber = payment.InvoiceNumber,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethod = payment.PaymentMethod,
                    PropertyName = lease.Property.PropertyName
                });
        }

        // Get payment by ID (tenant/owner)
        [HttpGet("{id}")]
        [Authorize(Roles = "tenant,owner")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.LeaseAgreement)
                    .ThenInclude(l => l.Property)
                .FirstOrDefaultAsync(p => p.PaymentID == id);

            if (payment == null) return NotFound();

            var userId = GetCurrentTenantIdFromToken();
            var userRole = GetCurrentUserRole();

            if (userRole == "tenant" && payment.LeaseAgreement.TenantID != userId)
                return Forbid();

            // Owner ID is still taken from ClaimTypes.NameIdentifier as per the original controller
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userRole == "owner" && payment.LeaseAgreement.Property.OwnerID != int.Parse(ownerIdClaim))
                return Forbid();

            return Ok(new PaymentHistoryDto
            {
                InvoiceNumber = payment.InvoiceNumber,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                PropertyName = payment.LeaseAgreement.Property.PropertyName
            });
        }

        [HttpGet("history/tenant")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetPaymentHistoryForTenant()
        {
            var tenantId = GetCurrentTenantIdFromToken();
            if (!tenantId.HasValue) return Unauthorized();

            var payments = await _context.Payments
                .Where(p => p.LeaseAgreement.TenantID == tenantId.Value)
                .Include(p => p.LeaseAgreement)
                    .ThenInclude(l => l.Property)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentHistoryDto
                {
                    InvoiceNumber = p.InvoiceNumber,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod,
                    PropertyName = p.LeaseAgreement.Property.PropertyName
                })
                .ToListAsync();

            return Ok(payments);
        }

        // Owner payment history for their properties (Owner ID still from ClaimTypes.NameIdentifier)
        private int? GetCurrentOwnerIdFromToken()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(ownerIdClaim, out var id) ? id : null;
        }

        [HttpGet("owner/details")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetOwnerPaymentDetails()
        {
            var ownerId = GetCurrentOwnerIdFromToken();
            if (!ownerId.HasValue)
            {
                return Unauthorized();
            }

            var paymentDetails = await _context.Payments
                .Include(p => p.LeaseAgreement)
                    .ThenInclude(l => l.Property)
                .Include(p => p.LeaseAgreement)
                    .ThenInclude(l => l.Tenant)
                .Where(p => p.LeaseAgreement.Property.OwnerID == ownerId.Value)
                .Select(p => new OwnerPaymentDetailDto // Create this DTO
                {
                    InvoiceNumber = p.InvoiceNumber,
                    PropertyName = p.LeaseAgreement.Property.PropertyName,
                    TenantName = p.LeaseAgreement.Tenant.Name,
                    AmountDue = p.Amount, // Or a calculated 'due' amount if applicable
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod,
                    // Assuming you have an enum or status field
                    // Add other relevant details
                })
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return Ok(paymentDetails);
        }
    


[HttpGet("notifications/tenant")]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetTenantLatePayments()
        {
            var tenantId = GetCurrentTenantIdFromToken();
            if (!tenantId.HasValue) return Unauthorized();

            var latePayments = await _context.LatePaymentNotifications
                .Include(n => n.Property) // Ensure Property navigation is included
                .Where(n => n.TenantID == tenantId.Value)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new LatePaymentNotificationDto
                {
                    NotificationID = n.NotificationID,
                    PropertyName = n.Property.PropertyName, // Include PropertyName
                    PropertyAddress = n.Property.Address,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                })
                .ToListAsync();

            return Ok(latePayments);
        }

        [HttpGet("notifications/owner")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetOwnerLatePayments()
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(ownerIdClaim, out var ownerId)) return Unauthorized();

            var latePayments = await _context.LatePaymentNotifications
                .Include(n => n.Tenant)
                .Include(n => n.Property) // Ensure Property navigation is included
                .Where(n => n.OwnerID == ownerId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new LatePaymentNotificationDto
                {
                    NotificationID = n.NotificationID,
                    TenantName = n.Tenant.Name,
                    PropertyName = n.Property.PropertyName, // Include PropertyName
                    PropertyAddress = n.Property.Address,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                })
                .ToListAsync();

            return Ok(latePayments);
        }

    }
}


