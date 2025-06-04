using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class RentalApplicationRepository : IRentalApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public RentalApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddRentalApplicationAsync(RentalApplication rentalApplication)
        {
            _context.RentalApplications.Add(rentalApplication);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RentalApplication>> GetAllRentalApplicationsAsync()
        {
            return await _context.RentalApplications.ToListAsync();
        }


        public async Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId)
        {
            return await _context.RentalApplications
                .Where(ra => ra.PropertyID == propertyId)
                .Include(ra => ra.Tenant)
                .ToListAsync();
        }

        public async Task<List<RentalApplication>> GetRentalApplicationsByTenantIdAsync(int tenantId)
        {
            return await _context.RentalApplications
                .Where(ra => ra.TenantID == tenantId)
                .Include(ra => ra.Property)
                .ToListAsync();
        }


        public async Task<List<RentalApplication>> GetRentalApplicationsByOwnerIdAsync(int ownerId)
        {
            return await _context.RentalApplications
                .Where(app => app.Property != null && app.Property.OwnerID == ownerId)
                .Include(app => app.Property)
                .ToListAsync();
        }


        public async Task<RentalApplication> GetRentalApplicationByIdAsync(int id)
        {
            return await _context.RentalApplications
                .Include(ra => ra.Property)
                .Include(ra => ra.Tenant)
                .FirstOrDefaultAsync(ra => ra.RentalApplicationID == id);
        }

        public async Task<bool> UpdateRentalApplicationStatusAsync(int id, string status)
        {
            var rentalApplication = await _context.RentalApplications.FindAsync(id);
            if (rentalApplication == null)
            {
                return false;
            }

            rentalApplication.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRentalApplicationAsync(RentalApplication rentalApplication)
        {
            _context.Entry(rentalApplication).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalApplicationExists(rentalApplication.RentalApplicationID))
                {
                    return false; // Or throw a NotFoundException
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteRentalApplicationAsync(int id)
        {
            var rentalApplication = await _context.RentalApplications.FindAsync(id);
            if (rentalApplication == null)
            {
                return false;
            }

            _context.RentalApplications.Remove(rentalApplication);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool RentalApplicationExists(int id)
        {
            return _context.RentalApplications.Any(e => e.RentalApplicationID == id);
        }
    }
}