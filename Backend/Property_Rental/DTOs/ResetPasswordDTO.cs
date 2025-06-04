using System.ComponentModel.DataAnnotations;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}