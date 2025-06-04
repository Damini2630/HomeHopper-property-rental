public class LatePaymentNotificationDto
{
    public int NotificationID { get; set; }
    public string TenantName { get; set; }
    public string PropertyAddress { get; set; }
    public string PropertyName { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DaysLate { get; set; }
}