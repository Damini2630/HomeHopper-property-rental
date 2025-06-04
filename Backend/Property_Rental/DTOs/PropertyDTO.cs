using System;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class PropertyDTO
    {
        public int PropertyID { get; set; } // Add this line
        public string PropertyName { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double RentAmount { get; set; }
        public bool AvailabilityStatus { get; set; }
        public string Amenities { get; set; }
        public string ImagePath { get; set; }
        public int OwnerID { get; set; }
        // You might also want to include CreatedAt if needed in the DTO
        // public DateTime CreatedAt { get; set; }
    }
}