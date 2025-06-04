

using Microsoft.EntityFrameworkCore;

using OnlineRentalPropertyManagement.Data;

using OnlineRentalPropertyManagement.Models;

using OnlineRentalPropertyManagement.Repositories.Interfaces;

using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;
 
namespace OnlineRentalPropertyManagement.Repositories

{

    public class OwnerRepository : IOwnerRepository

    {

        private readonly ApplicationDbContext _context;

        public OwnerRepository(ApplicationDbContext context)

        {

            _context = context;

        }

        public async Task<bool> AddOwnerAsync(Owner owner)

        {

            _context.Owners.Add(owner);

            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<Owner> GetOwnerByEmailAsync(string email)

        {

            return await _context.Owners

                .FirstOrDefaultAsync(o => o.Email == email);

        }

        public async Task<Owner> GetOwnerByIdAsync(int ownerId)

        {

            return await _context.Owners.FindAsync(ownerId);

        }

        public async Task<bool> UpdateOwnerAsync(Owner owner)

        {

            _context.Owners.Update(owner);

            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DeleteOwnerAsync(int ownerId)

        {

            var owner = await _context.Owners.FindAsync(ownerId);

            if (owner == null)

            {

                return false;

            }

            _context.Owners.Remove(owner);

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<List<RentalApplication>> GetRentalApplicationsByPropertyIdAsync(int propertyId)

        {

            return await _context.RentalApplications

                .Where(ra => ra.PropertyID == propertyId)

                .Include(ra => ra.Tenant)

                .ToListAsync();

        }

        public async Task<List<LeaseAgreement>> GetLeaseAgreementsByOwnerIdAsync(int ownerId)

        {

            return await _context.LeaseAgreements

                .Include(la => la.Property)

                .Where(la => la.Property.OwnerID == ownerId)

                .ToListAsync();

        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsByOwnerIdAsync(int ownerId)

        {

            return await _context.MaintenanceRequest

                .Include(mr => mr.Property)

                .Where(mr => mr.Property.OwnerID == ownerId)

                .ToListAsync();

        }

        public async Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int ownerId)

        {

            return await _context.LatePaymentNotifications

                .Where(n => n.OwnerID == ownerId && !n.IsRead)

                .ToListAsync();

        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)

        {

            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Email == email);

            if (owner == null) return false;

            owner.Password = newPassword;

            await _context.SaveChangesAsync();

            return true;

        }


    }

}


