//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineRentalPropertyManagement.Data;
//using OnlineRentalPropertyManagement.DTOs;
//using OnlineRentalPropertyManagement.Models;
//using OnlineRentalPropertyManagement.Services.Interfaces;
//using System;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;


namespace OnlineRentalPropertyManagement.Controllers
{
    internal class OwnerPaymentDetailDto
    {
        public string InvoiceNumber { get; set; }
        public string PropertyName { get; set; }
        public string TenantName { get; set; }
        public double AmountDue { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
    }
}