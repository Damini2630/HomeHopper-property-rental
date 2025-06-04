namespace OnlineRentalPropertyManagement.Models

{

    public class MaintenanceRequestDTO

    {

        public int RequestID { get; set; }

        public int PropertyID { get; set; }

        public int TenantID { get; set; }

        public int OwnerID { get; set; }

        public string IssueDescription { get; set; }

        public string Status { get; set; }

        public DateTime AssignedDate { get; set; }

    }

}
