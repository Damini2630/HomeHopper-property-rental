namespace OnlineRentalPropertyManagement.Models
{
    public class LatePaymentNotification
    {
        public int NotificationID { get; set; } // Primary Key
        public int TenantID { get; set; } // ID of the tenant
        public int OwnerID { get; set; } // ID of the owner
        public string Message { get; set; } // Notification message
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp
        public bool IsRead { get; set; } = false; // Whether the notification has been read

        // Navigation properties
        public Tenant Tenant { get; set; }
        public Property Property { get; set; }
        public int PropertyID { get; internal set; }
    }
}
