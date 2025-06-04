namespace OnlineRentalPropertyManagement.DTOs
{
    public class UpdatePropertyDTO
    {
        public string PropertyName { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double RentAmount { get; set; }
        public bool AvailabilityStatus { get; set; }
        public string Amenities { get; set; }
        public string ImagePath { get; set; } // Online image URL
    }
}
