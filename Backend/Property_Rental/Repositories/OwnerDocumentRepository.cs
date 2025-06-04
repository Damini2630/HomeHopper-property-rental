namespace OnlineRentalPropertyManagement.Repositories
{
    using global::OnlineRentalPropertyManagement.Data;
    using global::OnlineRentalPropertyManagement.Models;
    using global::OnlineRentalPropertyManagement.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    namespace OnlineRentalPropertyManagement.Repositories
    {
        public class OwnerDocumentRepository : IOwnerDocumentRepository
        {
            private readonly ApplicationDbContext _context;

            public OwnerDocumentRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<OwnerDocument> GetByLeaseIdAsync(int leaseId)
            {
                return await _context.OwnerDocuments.FirstOrDefaultAsync(od => od.LeaseID == leaseId);
            }

            public async Task<OwnerDocument> AddAsync(OwnerDocument entity)
            {
                _context.OwnerDocuments.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }

            public async Task UpdateAsync(OwnerDocument entity)
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}