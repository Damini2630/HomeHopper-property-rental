using System.ComponentModel.DataAnnotations;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class OwnerDocumentRequestDTO
    {
        [StringLength(255, ErrorMessage = "Owner signature path cannot exceed 255 characters.")]
        public string OwnerSignaturePath { get; set; }

        [StringLength(255, ErrorMessage = "Owner document path cannot exceed 255 characters.")]
        public string OwnerDocumentPath { get; set; }

        // The AcceptLease property is REMOVED
    }
}