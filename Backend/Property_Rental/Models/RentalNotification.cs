namespace OnlineRentalPropertyManagement.Models
{
    public class RentalNotification
    {
        public int NotificationID { get; set; } // Primary Key
        public int UserID { get; set; } // Foreign Key to User (Owner)
        public string Message { get; set; } // Notification message
        public bool IsRead { get; set; } // Whether the notification has been read
        public DateTime CreatedAt { get; set; } // Timestamp of the notification

        public Owner Owner { get; set; }
    }
}
