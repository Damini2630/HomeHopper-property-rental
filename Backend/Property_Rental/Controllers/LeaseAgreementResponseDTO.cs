using OnlineRentalPropertyManagement.DTOs;

namespace OnlineRentalPropertyManagement.Controllers
{
    internal class LeaseAgreementResponseDTO
    {
        public int LeaseID { get; set; }
        public int PropertyID { get; set; }
        public int TenantID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TenantSignaturePath { get; set; }
        public string TenantDocumentPath { get; set; }
        public string Status { get; set; }
        public double RentAmount { get; set; }
        public PropertyDTO Property { get; set; }
        public TenantDTO Tenant { get; set; }
        public OwnerDocumentDTO OwnerDocument { get; set; }
        public List<PaymentDTO> Payments { get; set; }
        public List<NotificationDTO> Notifications { get; set; }
    }
}