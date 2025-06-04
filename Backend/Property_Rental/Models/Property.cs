using System.Collections.Generic;

namespace OnlineRentalPropertyManagement.Models
{
    public class Property
    {
        internal readonly object Tenant;

        public int PropertyID { get; set; }
        public string PropertyName { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double RentAmount { get; set; }
        public bool AvailabilityStatus { get; set; }
        public string Amenities { get; set; }
        public string ImagePath { get; set; } // New field for image URL
        public int OwnerID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      

        // Navigation Properties
        public Owner Owner { get; set; }
        public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
        public ICollection<LeaseAgreement> LeaseAgreements { get; set; } = new List<LeaseAgreement>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequest { get; set; } = new List<MaintenanceRequest>();
        //public ICollection<LatePaymentNotification> LatePaymentNotifications { get; set; } = new List<LatePaymentNotification>();
        //public ICollection<Payment> Payment { get; set; } = new List<Payment>();
    }
}