using Microsoft.AspNetCore.Mvc;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Services.Interfaces;

namespace OnlineRentalPropertyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDTO adminLoginDTO)
        {
            var token = await _adminService.LoginAdminAsync(adminLoginDTO);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(new { Token = token });
        }
    }
}
