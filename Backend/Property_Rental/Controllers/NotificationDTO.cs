namespace OnlineRentalPropertyManagement.Controllers
{
    internal class NotificationDTO
    {
        public int NotificationID { get; set; }
        public int LeaseID { get; set; }
        public string Message { get; set; }
        public object SentDate { get; set; }
        public object IsRead { get; set; }
    }
}