using OnlineRentalPropertyManagement.DTOs;

namespace OnlineRentalPropertyManagement.Services.Interfaces
{
    public interface IAdminService
    {
        Task<string> LoginAdminAsync(AdminLoginDTO adminLoginDTO);
    }
}
