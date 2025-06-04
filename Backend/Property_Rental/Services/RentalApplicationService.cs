using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using OnlineRentalPropertyManagement.Services.Interfaces;

namespace OnlineRentalPropertyManagement.Services
{
    public class RentalApplicationService : IRentalApplicationService
    {
        private readonly IRentalApplicationRepository _rentalApplicationRepository;
        private readonly INotificationService _notificationService;

        public RentalApplicationService(IRentalApplicationRepository rentalApplicationRepository, INotificationService notificationService)
        {
            _rentalApplicationRepository = rentalApplicationRepository;
            _notificationService = notificationService;
        }


        public async Task<IEnumerable<RentalApplication>> GetAllRentalApplicationsAsync()
        {
            return await _rentalApplicationRepository.GetAllRentalApplicationsAsync();
        }


        public async Task<bool> SubmitRentalApplicationAsync(RentalApplicationDTO rentalApplicationDTO)
        {
            var rentalApplication = new RentalApplication
            {
                PropertyID = rentalApplicationDTO.PropertyID,
                TenantID = rentalApplicationDTO.TenantID,
                NoOfPeople = rentalApplicationDTO.NoOfPeople,
                StayPeriod = rentalApplicationDTO.StayPeriod,
                TentativeStartDate = rentalApplicationDTO.TentativeStartDate,
                PermanentAddress = rentalApplicationDTO.PermanentAddress,
                State = rentalApplicationDTO.State,
                Country = rentalApplicationDTO.Country,
                SpecificRequirements = rentalApplicationDTO.SpecificRequirements,
                ApplicationDate = DateTime.UtcNow,
                Status = "Pending"
            };

            return await _rentalApplicationRepository.AddRentalApplicationAsync(rentalApplication);
        }

        public async Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId)
        {
            return await _rentalApplicationRepository.GetRentalApplicationsByPropertyIdAsync(propertyId);
        }

        public async Task<List<RentalApplicationDTO>> GetRentalApplicationsByOwnerIdAsync(int ownerId)
        {
            var applications = await _rentalApplicationRepository.GetRentalApplicationsByOwnerIdAsync(ownerId);
            return applications.Select(app => new RentalApplicationDTO
            {
                RentalApplicationID = app.RentalApplicationID,
                PropertyID = app.PropertyID,
                TenantID = app.TenantID,
                NoOfPeople = app.NoOfPeople,
                StayPeriod = app.StayPeriod,
                TentativeStartDate = app.TentativeStartDate,
                PermanentAddress = app.PermanentAddress,
                State = app.State,
                Country = app.Country,
                SpecificRequirements = app.SpecificRequirements,
                ApplicationDate = app.ApplicationDate,
                Status = app.Status
            }).ToList();
        }

        public async Task<List<RentalApplication>> GetRentalApplicationsByTenantIdAsync(int tenantId)
        {
            return await _rentalApplicationRepository.GetRentalApplicationsByTenantIdAsync(tenantId);
        }

        public async Task<RentalApplication> GetRentalApplicationByIdAsync(int id)
        {
            return await _rentalApplicationRepository.GetRentalApplicationByIdAsync(id);
        }

        public async Task<bool> UpdateRentalApplicationStatusAsync(int id, string status)
        {
            return await _rentalApplicationRepository.UpdateRentalApplicationStatusAsync(id, status);
        }

        public async Task<bool> UpdateRentalApplicationAsync(RentalApplicationDTO rentalApplicationDTO)
        {
            var existingApplication = await _rentalApplicationRepository.GetRentalApplicationByIdAsync(rentalApplicationDTO.RentalApplicationID);

            if (existingApplication == null)
            {
                return false; // Or throw an exception: Application not found
            }

            // Update properties based on the DTO
            existingApplication.NoOfPeople = rentalApplicationDTO.NoOfPeople;
            existingApplication.StayPeriod = rentalApplicationDTO.StayPeriod;
            existingApplication.TentativeStartDate = rentalApplicationDTO.TentativeStartDate;
            existingApplication.PermanentAddress = rentalApplicationDTO.PermanentAddress;
            existingApplication.State = rentalApplicationDTO.State;
            existingApplication.Country = rentalApplicationDTO.Country;
            existingApplication.SpecificRequirements = rentalApplicationDTO.SpecificRequirements;
            // Do not update ApplicationDate or TenantID here, as they should remain the same

            return await _rentalApplicationRepository.UpdateRentalApplicationAsync(existingApplication);
        }

        public async Task<bool> DeleteRentalApplicationAsync(int id)
        {
            return await _rentalApplicationRepository.DeleteRentalApplicationAsync(id);
        }
    }
}