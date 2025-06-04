using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRentalPropertyManagement.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; } // Primary Key

        [Required]
        public int LeaseID { get; set; } // Foreign Key to LeaseAgreement

        [Required]
        public string Message { get; set; } // Notification message

        [Required]
        public DateTime DateCreated { get; set; } // Date the notification was created

        // Navigation Properties
        [ForeignKey("LeaseID")]
        public LeaseAgreement LeaseAgreement { get; set; }
    }
}
