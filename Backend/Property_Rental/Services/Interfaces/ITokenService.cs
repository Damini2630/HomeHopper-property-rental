using System.Security.Claims;

namespace OnlineRentalPropertyManagement.Services
{
    public interface ITokenService
    {
        string GenerateToken(Claim[] claims);
    }
}

