using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .ToListAsync();
        }

        public async Task<Property> GetPropertyByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PropertyID == id);
        }

        public async Task<Property> GetPropertyByNameAsync(string name)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PropertyName == name);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(int ownerId) // Changed parameter type to int
        {
            return await _context.Properties
                .Where(p => p.OwnerID == ownerId)
                .Include(p => p.Owner)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string state, string country)
        {
            var query = _context.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(state))
                query = query.Where(p => p.State.Contains(state));

            if (!string.IsNullOrEmpty(country))
                query = query.Where(p => p.Country.Contains(country));

            return await query.Include(p => p.Owner).ToListAsync();
        }

        public async Task<Property> AddPropertyAsync(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            _context.Entry(property).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return false;

            _context.Properties.Remove(property);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}