using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using OnlineRentalPropertyManagement.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all maintenance requests
        public async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
        {
            return await _context.MaintenanceRequest
                                .Include(mr => mr.Property) // Include related Property data
                                .Include(mr => mr.Tenant)   // Include related Tenant data
                                .ToListAsync();
        }

        // Get a maintenance request by ID
        public async Task<MaintenanceRequest> GetByIdAsync(int id)
        {
            var maintenanceRequest = await _context.MaintenanceRequest
                                                  .Include(mr => mr.Property) // Include related Property data
                                                  .Include(mr => mr.Tenant)   // Include related Tenant data
                                                  .FirstOrDefaultAsync(mr => mr.RequestID == id);

            if (maintenanceRequest == null)
            {
                throw new NotFoundException($"Maintenance request with ID {id} not found.");
            }

            return maintenanceRequest;
        }

        // Add a new maintenance request (using DTO)
        public async Task<MaintenanceRequest> SubmitMaintenanceRequest(int tenantId, int propertyId, string issueDescription)
        {
            // Ensure this method does not include unnecessary queries to the Property table
            var maintenanceRequest = new MaintenanceRequest
            {
                TenantID = tenantId,
                PropertyID = propertyId,
                IssueDescription = issueDescription,
                Status = "Pending",
                AssignedDate = DateTime.Now
            };

            _context.MaintenanceRequest.Add(maintenanceRequest);
            await _context.SaveChangesAsync();
            return maintenanceRequest;
        }
        // Add a new maintenance request (using entity)
        public async Task<MaintenanceRequest> SubmitMaintenanceRequest(MaintenanceRequest maintenanceRequest)
        {
            _context.MaintenanceRequest.Add(maintenanceRequest);
            await _context.SaveChangesAsync();
            return maintenanceRequest;
        }

        // Update an existing maintenance request
        public async Task<bool> UpdateAsync(int id, MaintenanceRequestDTO maintenanceRequestDTO)
        {
            var existingRequest = await _context.MaintenanceRequest.FindAsync(id);
            if (existingRequest == null)
            {
                throw new NotFoundException($"Maintenance request with ID {id} not found.");
            }

            // Update fields from DTO
            existingRequest.PropertyID = maintenanceRequestDTO.PropertyID;
            existingRequest.TenantID = maintenanceRequestDTO.TenantID;
            existingRequest.IssueDescription = maintenanceRequestDTO.IssueDescription;
            existingRequest.Status = maintenanceRequestDTO.Status;
            existingRequest.AssignedDate = maintenanceRequestDTO.AssignedDate;

            _context.Entry(existingRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MaintenanceRequestExistsAsync(id))
                {
                    throw new NotFoundException($"Maintenance request with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        // Delete a maintenance request
        public async Task<bool> DeleteAsync(int id)
        {
            var maintenanceRequest = await _context.MaintenanceRequest.FindAsync(id);
            if (maintenanceRequest == null)
            {
                throw new NotFoundException($"Maintenance request with ID {id} not found.");
            }

            _context.MaintenanceRequest.Remove(maintenanceRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if a maintenance request exists
        private async Task<bool> MaintenanceRequestExistsAsync(int id)
        {
            return await _context.MaintenanceRequest.AnyAsync(e => e.RequestID == id);
        }

        // Update an existing maintenance request (using entity)
        public async Task<bool> UpdateAsync(MaintenanceRequest maintenanceRequest)
        {
            _context.Entry(maintenanceRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task AddMaintenanceNotificationAsync(MaintenanceNotification notification)

        {

            _context.MaintenanceNotifications.Add(notification);

            await _context.SaveChangesAsync();

        }

        public async Task<List<MaintenanceNotification>> GetOwnerNotificationsAsync(int ownerId)

        {

            return await _context.MaintenanceNotifications

                .Where(n => n.tenantid == ownerId)

                .OrderByDescending(n => n.CreatedAt)

                .ToListAsync();

        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId)

        {

            var notification = await _context.MaintenanceNotifications.FindAsync(notificationId);

            if (notification == null) return false;

            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<IEnumerable<MaintenanceRequest>> GetRequestsByTenantIdAsync(int tenantId)
        {
            return await _context.MaintenanceRequest
                .Where(r => r.TenantID == tenantId)
                .ToListAsync();
        }


    }
}