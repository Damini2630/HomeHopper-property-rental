
namespace OnlineRentalPropertyManagement.Controllers
{
    internal class PaymentDTO
    {
        public int PaymentID { get; set; }
        public int LeaseID { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}