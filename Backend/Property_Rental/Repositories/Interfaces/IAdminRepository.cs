using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin> GetAdminByEmailAsync(string email);
    }
}
