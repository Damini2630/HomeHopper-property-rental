using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRentalPropertyManagement.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }

        [Required]
        public int LeaseID { get; set; }

        [Required]
        
        public double Amount { get; set; }  // Changed from double to decimal

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string InvoiceNumber { get; set; } = GenerateInvoiceNumber();

        [ForeignKey("LeaseID")]
        public virtual LeaseAgreement LeaseAgreement { get; set; }

        private static string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }
    }
}