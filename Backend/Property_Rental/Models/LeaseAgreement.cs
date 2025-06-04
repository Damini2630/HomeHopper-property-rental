using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRentalPropertyManagement.Models
{
    public class LeaseAgreement
    {
        [Key]
        public int LeaseID { get; set; } // Primary Key

        [Required]
        public int PropertyID { get; set; } // Foreign Key to Property

        [Required]
        public int TenantID { get; set; } // Foreign Key to Tenant

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(255)]
        public string TenantSignaturePath { get; set; } // Path to tenant's signature file

        [Required]
        [StringLength(255)]
        public string TenantDocumentPath { get; set; } // Path to tenant's uploaded document

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // e.g., Pending, Signed, Executed

        // Navigation Properties
        [ForeignKey("PropertyID")]
        public Property Property { get; set; }

        [ForeignKey("TenantID")]
        public Tenant Tenant { get; set; }

        public OwnerDocument OwnerDocument { get; set; } // Navigation property for OwnerDocument

        public ICollection<Payment> Payments { get; set; } // Navigation property for Payments
        public ICollection<Notification> Notifications { get; set; } // Navigation property for Notifications

        [NotMapped]
        public double RentAmount => Property?.RentAmount ?? 0;
    }
}
