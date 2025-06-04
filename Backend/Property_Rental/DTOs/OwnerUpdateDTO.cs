namespace OnlineRentalPropertyManagement.DTOs
{
    public class OwnerUpdateDTO
    {
        public int OwnerId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string ContactDetails { get; set; }
        public string Password { get; set; }

        // Add other properties as needed
    }
}