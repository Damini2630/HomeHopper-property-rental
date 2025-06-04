using System;

namespace OnlineRentalPropertyManagement.DTOs
{
    public class LeaseAgreementResponseDTO
    {
        public int LeaseID { get; set; }
        public int PropertyID { get; set; }
        public int TenantID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TenantSignaturePath { get; set; }
        public string TenantDocumentPath { get; set; }
        public string Status { get; set; }
        // Include relevant details from Property and Tenant if needed
        public PropertyInfoDTO Property { get; set; }
        public TenantInfoDTO Tenant { get; set; }
        public OwnerDocumentInfoDTO OwnerDocument { get; set; }
        // Add other relevant properties
    }

    public class PropertyInfoDTO
    {
        public int PropertyID { get; set; }
        public string Address { get; set; }
        public decimal RentAmount { get; set; }
        // Add other relevant property details
    }

    public class TenantInfoDTO
    {
        public int TenantID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Add other relevant tenant details
    }

    public class OwnerDocumentInfoDTO
    {
        public int OwnerDocumentID { get; set; }
        public string OwnerSignaturePath { get; set; }
        public string OwnerDocumentPath { get; set; }
        // Add other relevant owner document details
    }
}