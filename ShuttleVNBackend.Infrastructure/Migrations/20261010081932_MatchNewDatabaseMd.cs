using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchNewDatabaseMd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingStatusHistories_Employees_ChangedByEmployee",
                table: "BookingStatusHistories");

            migrationBuilder.RenameColumn(
                name: "ChangedByEmployee",
                table: "BookingStatusHistories",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingStatusHistories_ChangedByEmployee",
                table: "BookingStatusHistories",
                newName: "IX_BookingStatusHistories_EmployeeId");

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "UserAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CancellationActorType",
                table: "Invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByCustomer",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByEmployee",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaidByEmployee",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Invoices",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundMethod",
                table: "Invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RefundedByEmployee",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacesInvoiceId",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorType",
                table: "BookingStatusHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "BookingStatusHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BadmintonCourts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "BadmintonCourts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Audits",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "BookingPriceSegments",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SegmentNo = table.Column<int>(type: "integer", nullable: false),
                    PriceStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    SegmentStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    SegmentEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPriceSegments", x => new { x.BookingId, x.SegmentNo });
                    table.ForeignKey(
                        name: "FK_BookingPriceSegments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdempotencyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorScope = table.Column<string>(type: "text", nullable: false),
                    RequestKey = table.Column<string>(type: "text", nullable: false),
                    Operation = table.Column<string>(type: "text", nullable: false),
                    RequestHash = table.Column<string>(type: "text", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResponseStatus = table.Column<int>(type: "integer", nullable: false),
                    ResponseBody = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDailyCounters",
                columns: table => new
                {
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LastNumber = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDailyCounters", x => x.IssueDate);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventKey = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientEmail = table.Column<string>(type: "text", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    NextAttemptAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistories_CustomerId",
                table: "BookingStatusHistories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequests_ActorScope_RequestKey",
                table: "IdempotencyRequests",
                columns: new[] { "ActorScope", "RequestKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EventKey",
                table: "Notifications",
                column: "EventKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingStatusHistories_Customers_CustomerId",
                table: "BookingStatusHistories",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingStatusHistories_Employees_EmployeeId",
                table: "BookingStatusHistories",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingStatusHistories_Customers_CustomerId",
                table: "BookingStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingStatusHistories_Employees_EmployeeId",
                table: "BookingStatusHistories");

            migrationBuilder.DropTable(
                name: "BookingPriceSegments");

            migrationBuilder.DropTable(
                name: "IdempotencyRequests");

            migrationBuilder.DropTable(
                name: "InvoiceDailyCounters");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_BookingStatusHistories_CustomerId",
                table: "BookingStatusHistories");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "CancellationActorType",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CancelledByCustomer",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CancelledByEmployee",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaidByEmployee",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RefundMethod",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RefundedByEmployee",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ReplacesInvoiceId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ActorType",
                table: "BookingStatusHistories");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "BookingStatusHistories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BadmintonCourts");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "BookingStatusHistories",
                newName: "ChangedByEmployee");

            migrationBuilder.RenameIndex(
                name: "IX_BookingStatusHistories_EmployeeId",
                table: "BookingStatusHistories",
                newName: "IX_BookingStatusHistories_ChangedByEmployee");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BadmintonCourts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Audits",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingStatusHistories_Employees_ChangedByEmployee",
                table: "BookingStatusHistories",
                column: "ChangedByEmployee",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }
    }
}
