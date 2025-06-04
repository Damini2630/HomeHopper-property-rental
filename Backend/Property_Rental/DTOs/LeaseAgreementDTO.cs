using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class LeaseAgreementDTO
    {
        [Required(ErrorMessage = "Property ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Property ID must be a positive number.")]
        public int PropertyID { get; set; }

        [Required(ErrorMessage = "Tenant ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tenant ID must be a positive number.")]
        public int TenantID { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Tenant signature path is required.")]
        [StringLength(255, ErrorMessage = "Tenant signature path cannot exceed 255 characters.")]
        public string TenantSignaturePath { get; set; }

        [Required(ErrorMessage = "Tenant document path is required.")]
        [StringLength(255, ErrorMessage = "Tenant document path cannot exceed 255 characters.")]
        public string TenantDocumentPath { get; set; }
    }
}

