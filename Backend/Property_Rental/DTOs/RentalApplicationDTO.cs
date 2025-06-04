using System;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class RentalApplicationDTO
    {
        public int RentalApplicationID { get; set; } // Or public int RentalApplicationID { get; set; } - choose one for consistency
        public int PropertyID { get; set; }
        public int TenantID { get; set; }
        public int NoOfPeople { get; set; }
        public string StayPeriod { get; set; }
        public DateTime TentativeStartDate { get; set; }
        public string PermanentAddress { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string SpecificRequirements { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }
    }
}