using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
//using OnlineRentalPropertyManagement.Models.OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _context;

        public TenantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTenantAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Tenant> GetTenantByEmailAsync(string email)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Email == email);
        }

        public async Task<List<LeaseAgreement>> GetLeaseAgreementsByTenantIdAsync(int tenantId)
        {
            return await _context.LeaseAgreements
                .Include(la => la.Property)
                .Where(la => la.TenantID == tenantId)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsByTenantIdAsync(int tenantId)
        {
            return await _context.MaintenanceRequest
                .Include(mr => mr.Property)
                .Where(mr => mr.TenantID == tenantId)
                .ToListAsync();
        }
        public async Task<IEnumerable<LatePaymentNotification>> GetLatePaymentNotificationsAsync(int tenantId)
        {
            return await _context.LatePaymentNotifications
                .Where(n => n.TenantID == tenantId && !n.IsRead)
                .ToListAsync();
        }
        public async Task<List<RentalApplication>> CheckApplicationStatusAsync(int applicationId)
        {
            var application = await _context.RentalApplications.FindAsync(applicationId);
            //return application?.Status;

            return await _context.RentalApplications
                .Include(rs => rs.Tenant)
                .Include(rs => rs.Property)
                .Where(rs => rs.RentalApplicationID == applicationId)
                .ToListAsync();
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Email == email);
            if (tenant == null) return false;

            tenant.Password = newPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTenantAsync(Tenant tenant)
        {
            _context.Tenants.Update(tenant);
            return await _context.SaveChangesAsync() > 0;
        }


    }
}