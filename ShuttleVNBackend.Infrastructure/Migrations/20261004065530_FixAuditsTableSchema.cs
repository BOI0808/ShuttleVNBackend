using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAuditsTableSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActorId",
                table: "Audits",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_ActorId",
                table: "Audits",
                newName: "IX_Audits_EmployeeId");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Audits",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CustomerId",
                table: "Audits",
                column: "CustomerId");

            migrationBuilder.AddCheckConstraint(
                name: "ck_audit_one_actor",
                table: "Audits",
                sql: "\"EmployeeId\" IS NULL OR \"CustomerId\" IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_Customers_CustomerId",
                table: "Audits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_Employees_EmployeeId",
                table: "Audits",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_Customers_CustomerId",
                table: "Audits");

            migrationBuilder.DropForeignKey(
                name: "FK_Audits_Employees_EmployeeId",
                table: "Audits");

            migrationBuilder.DropIndex(
                name: "IX_Audits_CustomerId",
                table: "Audits");

            migrationBuilder.DropCheckConstraint(
                name: "ck_audit_one_actor",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Audits",
                newName: "ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_EmployeeId",
                table: "Audits",
                newName: "IX_Audits_ActorId");
        }
    }
}
