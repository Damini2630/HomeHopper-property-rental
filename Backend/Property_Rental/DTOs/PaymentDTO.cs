using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class PaymentDto
    {
        [Required]
        public int LeaseID { get; set; }

        [Required]
        // Remove the Range attribute if you don't want a minimum constraint
        // [Range(0.01, double.MaxValue)]
        public double Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }
    }

    public class PaymentHistoryDto
    {
        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PropertyName { get; set; }
    }
}