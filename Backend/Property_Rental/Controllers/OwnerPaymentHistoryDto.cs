
namespace OnlineRentalPropertyManagement.Controllers
{
    internal class OwnerPaymentHistoryDto
    {
        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PropertyName { get; set; }
        public string TenantName { get; set; }
    }
}