using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using OnlineRentalPropertyManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;

        public MaintenanceService(IMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<int> SubmitMaintenanceRequest(MaintenanceRequest maintenanceRequest)
        {
            maintenanceRequest.Status = "Pending";
            maintenanceRequest.AssignedDate = DateTime.UtcNow;
            var createdRequest = await _maintenanceRepository.SubmitMaintenanceRequest(maintenanceRequest);
            return createdRequest.RequestID;
        }

        public async Task<List<MaintenanceRequest>> GetPendingRequestsForOwner(int ownerId)
        {
            var allRequests = await _maintenanceRepository.GetAllAsync();
            return allRequests
                .Where(r => r.Property.OwnerID == ownerId && r.Status == "Pending")
                .ToList();
        }

        public async Task<bool> UpdateRequestStatus(int requestId, string status)
        {
            var existingRequest = await _maintenanceRepository.GetByIdAsync(requestId);
            if (existingRequest == null)
            {
                return false;
            }

            existingRequest.Status = status;
            return await _maintenanceRepository.UpdateAsync(existingRequest);
        }

        public async Task<bool> GenerateBill(int serviceRequestId, double amount)
        {
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetAllMaintenanceRequestsAsync()
        {
            return await _maintenanceRepository.GetAllAsync();
        }

        public async Task<MaintenanceRequestDTO?> GetMaintenanceRequestByIdAsync(int id)
        {
            var request = await _maintenanceRepository.GetByIdAsync(id);
            if (request == null) return null;

            return new MaintenanceRequestDTO
            {
                RequestID = request.RequestID,
                PropertyID = request.PropertyID,
                TenantID = request.TenantID,
                OwnerID = request.OwnerID,
                IssueDescription = request.IssueDescription,
                Status = request.Status,
                AssignedDate = request.AssignedDate
            };
        }

        public async Task<MaintenanceRequest> CreateMaintenanceRequestAsync(MaintenanceRequestDTO maintenanceRequestDTO)
        {
            var maintenanceRequest = new MaintenanceRequest
            {
                PropertyID = maintenanceRequestDTO.PropertyID,
                TenantID = maintenanceRequestDTO.TenantID,
                OwnerID = maintenanceRequestDTO.OwnerID,
                IssueDescription = maintenanceRequestDTO.IssueDescription,
                Status = maintenanceRequestDTO.Status,
                AssignedDate = maintenanceRequestDTO.AssignedDate
            };

            return await _maintenanceRepository.SubmitMaintenanceRequest(maintenanceRequest);
        }

        public async Task<bool> UpdateMaintenanceRequestAsync(int id, MaintenanceRequestDTO maintenanceRequestDTO)
        {
            var existingRequest = await _maintenanceRepository.GetByIdAsync(id);
            if (existingRequest == null)
            {
                return false;
            }

            existingRequest.PropertyID = maintenanceRequestDTO.PropertyID;
            existingRequest.TenantID = maintenanceRequestDTO.TenantID;
            existingRequest.OwnerID = maintenanceRequestDTO.OwnerID;
            existingRequest.IssueDescription = maintenanceRequestDTO.IssueDescription;
            existingRequest.Status = maintenanceRequestDTO.Status;
            existingRequest.AssignedDate = maintenanceRequestDTO.AssignedDate;

            return await _maintenanceRepository.UpdateAsync(existingRequest);
        }

        public async Task<bool> DeleteMaintenanceRequestAsync(int id)
        {
            return await _maintenanceRepository.DeleteAsync(id);
        }

        public async Task AddOwnerNotificationAsync(MaintenanceNotification notification)
        {
            await _maintenanceRepository.AddMaintenanceNotificationAsync(notification);
        }

        public async Task AddTenantNotificationAsync(MaintenanceNotification notification)
        {
            await _maintenanceRepository.AddMaintenanceNotificationAsync(notification);
        }

        public async Task<List<MaintenanceNotification>> GetOwnerNotificationsAsync(int ownerId)
        {
            return await _maintenanceRepository.GetOwnerNotificationsAsync(ownerId);
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
        {
            return await _maintenanceRepository.MarkNotificationAsReadAsync(notificationId);
        }

        public async Task<IEnumerable<MaintenanceRequestDTO>> GetRequestsByTenantIdAsync(int tenantId)
        {
            var requests = await _maintenanceRepository.GetRequestsByTenantIdAsync(tenantId);

            return requests.Select(r => new MaintenanceRequestDTO
            {
                RequestID = r.RequestID,
                PropertyID = r.PropertyID,
                TenantID = r.TenantID,
                OwnerID = r.OwnerID,
                IssueDescription = r.IssueDescription,
                Status = r.Status,
                AssignedDate = r.AssignedDate
            });
        }
    }
}
    
    
    