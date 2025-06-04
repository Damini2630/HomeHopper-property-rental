using System;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRentalPropertyManagement.Models

{

    public class Servicerequest

    {

        [Key]

        public int ServiceID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int AgentID { get; set; }

        [Required]

        [StringLength(100)]

        public string AgentName { get; set; } // Added AgentName property

        [Required]

        [StringLength(15)]

        public string AgentContactNo { get; set; } // Added AgentContactNo property

        [ForeignKey("Request")]

        public int RequestID { get; set; }

        [Required]

        [StringLength(20)]

        public String Status { get; set; } // Renamed to ServiceStatus

        [Required]

        [Range(0, double.MaxValue)]

        public double ServiceBill { get; set; } // Added ServiceBill property

        public virtual MaintenanceRequest Request { get; set; }

    }

}
