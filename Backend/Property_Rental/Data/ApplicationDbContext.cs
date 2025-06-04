using Microsoft.EntityFrameworkCore;
using OnlineRentalPropertyManagement.Models;

namespace OnlineRentalPropertyManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MaintenanceRequest> MaintenanceRequest { get; set; }
        public DbSet<Servicerequest> Servicerequests { get; set; }
        public DbSet<LeaseAgreement> LeaseAgreements { get; set; }
        public DbSet<OwnerDocument> OwnerDocuments { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<RentalApplication> RentalApplications { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<MaintenanceNotification> MaintenanceNotifications { get; set; }
        public DbSet<LatePaymentNotification> LatePaymentNotifications { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<RentalNotification> RentalNotifications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed the default admin
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                AdminID = 1,
                Email = "admin@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123")
            });

            // Configure primary key for LeaseAgreement (Redundant, but good to be explicit)
            modelBuilder.Entity<LeaseAgreement>()
                .HasKey(la => la.LeaseID);

            // Configure relationships for Property
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(o => o.Properties)
                .HasForeignKey(p => p.OwnerID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for RentalApplication
            modelBuilder.Entity<RentalApplication>()
                .HasOne(ra => ra.Property)
                .WithMany(p => p.RentalApplications)
                .HasForeignKey(ra => ra.PropertyID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RentalApplication>()
                .HasOne(ra => ra.Tenant)
                .WithMany(t => t.RentalApplications)
                .HasForeignKey(ra => ra.TenantID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for LeaseAgreement
            modelBuilder.Entity<LeaseAgreement>()
                .HasOne(la => la.Property)
                .WithMany(p => p.LeaseAgreements)
                .HasForeignKey(la => la.PropertyID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaseAgreement>()
                .HasOne(la => la.Tenant)
                .WithMany(t => t.LeaseAgreements)
                .HasForeignKey(la => la.TenantID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.LeaseAgreement)
                .WithMany(la => la.Payments)
                .HasForeignKey(p => p.LeaseID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for MaintenanceRequest
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Property)
                .WithMany(p => p.MaintenanceRequest)
                .HasForeignKey(mr => mr.PropertyID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Tenant)
                .WithMany(t => t.MaintenanceRequest)
                .HasForeignKey(mr => mr.TenantID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Owner)
                .WithMany(o => o.MaintenanceRequest)
                .HasForeignKey(mr => mr.OwnerID)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure relationships for Servicerequest
            modelBuilder.Entity<Servicerequest>()
                .Property(ms => ms.Status)
                .HasMaxLength(20);

            modelBuilder.Entity<Servicerequest>()
                .HasCheckConstraint("CK_Status", "[Status] IN ('Pending', 'InProgress', 'Completed')");

            modelBuilder.Entity<Servicerequest>()
                .HasOne(ms => ms.Request)
                .WithMany()
                .HasForeignKey(ms => ms.RequestID);
            modelBuilder.Entity<OwnerDocument>()
        .Property(od => od.OwnerDocumentPath)
        .IsRequired(); // This line is crucial!

            // Configure one-to-one relationship between LeaseAgreement and OwnerDocument
            modelBuilder.Entity<LeaseAgreement>()
                .HasOne(la => la.OwnerDocument)
                .WithOne(od => od.LeaseAgreement)
                .HasForeignKey<OwnerDocument>(od => od.LeaseID);

           

            // Configure value generation for Servicerequest
            //modelBuilder.Entity<Servicerequest>()
            //    .Property(sr => sr.ServiceID)
            //    .ValueGeneratedOnAdd();

            modelBuilder.Entity<Servicerequest>()
                .Property(sr => sr.AgentID)
                .ValueGeneratedNever();

            // Configure primary keys for Notification types (Important!)
            modelBuilder.Entity<MaintenanceNotification>()
                .HasKey(n => n.NotificationID);
            modelBuilder.Entity<LatePaymentNotification>()
                .HasKey(n => n.NotificationID);
            modelBuilder.Entity<RentalNotification>()
                .HasKey(n => n.NotificationID);
        }
    }
}