using System.ComponentModel.DataAnnotations;

namespace OnlineRentalPropertyManagement.DTOs

{

    public class ServiceRequestDTO

    {

        [Required]

        public int RequestID { get; set; } // Foreign Key to MaintenanceRequest

        [Required]

        [StringLength(100)]

        public string AgentName { get; set; }

        [Required]

        [StringLength(15)]

        public string AgentContactNo { get; set; }

        [Required]

        [StringLength(20)]

        public string Status { get; set; } // "Pending", "In Progress", "Completed"

        [Required]

        public double ServiceBill { get; set; } // If not needed later, set default to 0

    }

}
