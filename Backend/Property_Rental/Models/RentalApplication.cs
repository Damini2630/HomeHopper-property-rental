using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace OnlineRentalPropertyManagement.Models
{
    public class RentalApplication
    {
        public int RentalApplicationID { get; set; }
        public int PropertyID { get; set; }
        public int TenantID { get; set; }
        public int NoOfPeople { get; set; }
        public string StayPeriod { get; set; } // e.g., "6 months", "1 year"
        public DateTime TentativeStartDate { get; set; }
        public string PermanentAddress { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string SpecificRequirements { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected

        // Navigation Properties
        public Property Property { get; set; }
        public Tenant Tenant { get; set; }
    }
}
