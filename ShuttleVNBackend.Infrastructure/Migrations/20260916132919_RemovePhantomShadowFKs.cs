using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhantomShadowFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingStatusHistories_Employees_ChangedBy",
                table: "BookingStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Employees_IssuedBy",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Customers_CustomerId",
                table: "UserAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Employees_EmployeeId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_CustomerId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_EmployeeId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "UserAccounts");

            migrationBuilder.RenameColumn(
                name: "IssuedBy",
                table: "Invoices",
                newName: "IssuedByEmployee");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_IssuedBy",
                table: "Invoices",
                newName: "IX_Invoices_IssuedByEmployee");

            migrationBuilder.RenameColumn(
                name: "ChangedBy",
                table: "BookingStatusHistories",
                newName: "ChangedByEmployee");

            migrationBuilder.RenameIndex(
                name: "IX_BookingStatusHistories_ChangedBy",
                table: "BookingStatusHistories",
                newName: "IX_BookingStatusHistories_ChangedByEmployee");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingStatusHistories_Employees_ChangedByEmployee",
                table: "BookingStatusHistories",
                column: "ChangedByEmployee",
                principalTable: "Employees",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Employees_IssuedByEmployee",
                table: "Invoices",
                column: "IssuedByEmployee",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingStatusHistories_Employees_ChangedByEmployee",
                table: "BookingStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Employees_IssuedByEmployee",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "IssuedByEmployee",
                table: "Invoices",
                newName: "IssuedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_IssuedByEmployee",
                table: "Invoices",
                newName: "IX_Invoices_IssuedBy");

            migrationBuilder.RenameColumn(
                name: "ChangedByEmployee",
                table: "BookingStatusHistories",
                newName: "ChangedBy");

            migrationBuilder.RenameIndex(
                name: "IX_BookingStatusHistories_ChangedByEmployee",
                table: "BookingStatusHistories",
                newName: "IX_BookingStatusHistories_ChangedBy");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "UserAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "UserAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_CustomerId",
                table: "UserAccounts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_EmployeeId",
                table: "UserAccounts",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingStatusHistories_Employees_ChangedBy",
                table: "BookingStatusHistories",
                column: "ChangedBy",
                principalTable: "Employees",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Employees_IssuedBy",
                table: "Invoices",
                column: "IssuedBy",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Customers_CustomerId",
                table: "UserAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Employees_EmployeeId",
                table: "UserAccounts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }
    }
}
