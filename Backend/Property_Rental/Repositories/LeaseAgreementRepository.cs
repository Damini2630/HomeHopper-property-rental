using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class LeaseAgreementRepository : ILeaseAgreementRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaseAgreementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaseAgreement> AddAsync(LeaseAgreement entity)
        {
            _context.LeaseAgreements.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                // Log the outer exception
                Console.WriteLine($"Error saving LeaseAgreement: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
                throw; // Re-throw the exception so the controller can handle it
            }
            catch (Exception ex)
            {
                // Log any other unexpected errors
                Console.WriteLine($"An unexpected error occurred in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<LeaseAgreement> GetByIdAsync(int id)
        {
            return await _context.LeaseAgreements
                .Include(la => la.Property)
                    .ThenInclude(p => p.Owner)
                .Include(la => la.Tenant)
                .Include(la => la.OwnerDocument)
                .FirstOrDefaultAsync(la => la.LeaseID == id);
        }

        public async Task<List<LeaseAgreement>> GetAllAsync()
        {
            return await _context.LeaseAgreements
                .Include(la => la.Property)
                .Include(la => la.Tenant)
                .Include(la => la.OwnerDocument)
                .ToListAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var leaseAgreement = await _context.LeaseAgreements.FindAsync(id);
            if (leaseAgreement != null)
            {
                _context.LeaseAgreements.Remove(leaseAgreement);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error deleting LeaseAgreement with ID {id}: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred in DeleteAsync for ID {id}: {ex.Message}");
                    throw;
                }
            }
            // If the entity is not found, no exception is thrown as the operation is technically "successful" in that the record is not there.
        }
    
        public async Task<List<LeaseAgreement>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.LeaseAgreements
                .Where(la => la.TenantID == tenantId)
                .Include(la => la.Property)
                .OrderByDescending(la => la.StartDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(LeaseAgreement entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}