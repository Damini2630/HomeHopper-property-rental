using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Data;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Repositories.Interfaces;
using System;

namespace OnlineRentalPropertyManagement.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            return await _context.Admins.SingleOrDefaultAsync(a => a.Email == email);
        }
    }
}
