namespace OnlineRentalPropertyManagement.Models
{
    public class RentalApplicationNotification
    {
        //public int NotificationID { get; set; } // Primary Key
        public int TenantID { get; set; } // ID of the user (tenant or owner)
        public string Message { get; set; } // Notification message
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp
        public bool IsRead { get; set; } = false; // Whether the notification has been read
    }
}