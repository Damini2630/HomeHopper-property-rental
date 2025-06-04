using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineRentalPropertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminID);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerID);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantID);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropertyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentAmount = table.Column<double>(type: "float", nullable: false),
                    AvailabilityStatus = table.Column<bool>(type: "bit", nullable: false),
                    Amenities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropertyID);
                    table.ForeignKey(
                        name: "FK_Properties_Owners_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Owners",
                        principalColumn: "OwnerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalNotifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalNotifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_RentalNotifications_Owners_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Owners",
                        principalColumn: "OwnerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LatePaymentNotifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantID = table.Column<int>(type: "int", nullable: false),
                    OwnerID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    PropertyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatePaymentNotifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_LatePaymentNotifications_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LatePaymentNotifications_Tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenants",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaseAgreements",
                columns: table => new
                {
                    LeaseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    TenantID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantSignaturePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenantDocumentPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseAgreements", x => x.LeaseID);
                    table.ForeignKey(
                        name: "FK_LeaseAgreements_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaseAgreements_Tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenants",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequest",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    TenantID = table.Column<int>(type: "int", nullable: false),
                    OwnerID = table.Column<int>(type: "int", nullable: false),
                    IssueDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRequest", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Owners_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Owners",
                        principalColumn: "OwnerID");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenants",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "RentalApplications",
                columns: table => new
                {
                    RentalApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    TenantID = table.Column<int>(type: "int", nullable: false),
                    NoOfPeople = table.Column<int>(type: "int", nullable: false),
                    StayPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TentativeStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecificRequirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalApplications", x => x.RentalApplicationID);
                    table.ForeignKey(
                        name: "FK_RentalApplications_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalApplications_Tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenants",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaseID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notification_LeaseAgreements_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "LeaseAgreements",
                        principalColumn: "LeaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnerDocuments",
                columns: table => new
                {
                    OwnerDocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaseID = table.Column<int>(type: "int", nullable: false),
                    OwnerSignaturePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerDocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerDocuments", x => x.OwnerDocumentID);
                    table.ForeignKey(
                        name: "FK_OwnerDocuments_LeaseAgreements_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "LeaseAgreements",
                        principalColumn: "LeaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaseID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payments_LeaseAgreements_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "LeaseAgreements",
                        principalColumn: "LeaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceNotifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenantid = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    MaintenanceRequestRequestID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceNotifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_MaintenanceNotifications_MaintenanceRequest_MaintenanceRequestRequestID",
                        column: x => x.MaintenanceRequestRequestID,
                        principalTable: "MaintenanceRequest",
                        principalColumn: "RequestID");
                });

            migrationBuilder.CreateTable(
                name: "Servicerequests",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentID = table.Column<int>(type: "int", nullable: false),
                    AgentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AgentContactNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ServiceBill = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicerequests", x => x.ServiceID);
                    table.CheckConstraint("CK_Status", "[Status] IN ('Pending', 'InProgress', 'Completed')");
                    table.ForeignKey(
                        name: "FK_Servicerequests_MaintenanceRequest_RequestID",
                        column: x => x.RequestID,
                        principalTable: "MaintenanceRequest",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminID", "Email", "Password" },
                values: new object[] { 1, "admin@gmail.com", "$2a$11$TdZLZnX3tmxdC0DiDFqBfuWImrUYOq6LNIykxAMFIB1p4WdH7VQlm" });

            migrationBuilder.CreateIndex(
                name: "IX_LatePaymentNotifications_PropertyID",
                table: "LatePaymentNotifications",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_LatePaymentNotifications_TenantID",
                table: "LatePaymentNotifications",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseAgreements_PropertyID",
                table: "LeaseAgreements",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseAgreements_TenantID",
                table: "LeaseAgreements",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceNotifications_MaintenanceRequestRequestID",
                table: "MaintenanceNotifications",
                column: "MaintenanceRequestRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_OwnerID",
                table: "MaintenanceRequest",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_PropertyID",
                table: "MaintenanceRequest",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_TenantID",
                table: "MaintenanceRequest",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_LeaseID",
                table: "Notification",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerDocuments_LeaseID",
                table: "OwnerDocuments",
                column: "LeaseID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LeaseID",
                table: "Payments",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_OwnerID",
                table: "Properties",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_PropertyID",
                table: "RentalApplications",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_TenantID",
                table: "RentalApplications",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalNotifications_OwnerID",
                table: "RentalNotifications",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Servicerequests_RequestID",
                table: "Servicerequests",
                column: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "LatePaymentNotifications");

            migrationBuilder.DropTable(
                name: "MaintenanceNotifications");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OwnerDocuments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RentalApplications");

            migrationBuilder.DropTable(
                name: "RentalNotifications");

            migrationBuilder.DropTable(
                name: "Servicerequests");

            migrationBuilder.DropTable(
                name: "LeaseAgreements");

            migrationBuilder.DropTable(
                name: "MaintenanceRequest");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Owners");
        }
    }
}
