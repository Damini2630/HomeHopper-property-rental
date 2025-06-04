using System.ComponentModel.DataAnnotations;

public class TenantRegistrationDTO
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email must be in a valid format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{10,}$",
        ErrorMessage = "Password must be at least 10 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Contact details are required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact details must be a 10-digit phone number.")]
    public string ContactDetails { get; set; }
}