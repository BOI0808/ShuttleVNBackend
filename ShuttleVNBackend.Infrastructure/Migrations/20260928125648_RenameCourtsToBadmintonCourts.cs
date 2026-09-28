using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCourtsToBadmintonCourts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtSchedules_Courts_CourtId",
                table: "CourtSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PricingRules_Courts_CourtId",
                table: "PricingRules");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courts",
                table: "Courts");

            migrationBuilder.RenameTable(
                name: "Courts",
                newName: "BadmintonCourts");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "Employees",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BadmintonCourts",
                table: "BadmintonCourts",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BadmintonCourts_CourtId",
                table: "Bookings",
                column: "CourtId",
                principalTable: "BadmintonCourts",
                principalColumn: "CourtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtSchedules_BadmintonCourts_CourtId",
                table: "CourtSchedules",
                column: "CourtId",
                principalTable: "BadmintonCourts",
                principalColumn: "CourtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees",
                column: "AccountId",
                principalTable: "UserAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PricingRules_BadmintonCourts_CourtId",
                table: "PricingRules",
                column: "CourtId",
                principalTable: "BadmintonCourts",
                principalColumn: "CourtId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BadmintonCourts_CourtId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtSchedules_BadmintonCourts_CourtId",
                table: "CourtSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PricingRules_BadmintonCourts_CourtId",
                table: "PricingRules");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BadmintonCourts",
                table: "BadmintonCourts");

            migrationBuilder.RenameTable(
                name: "BadmintonCourts",
                newName: "Courts");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "Employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courts",
                table: "Courts",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "CourtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtSchedules_Courts_CourtId",
                table: "CourtSchedules",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "CourtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees",
                column: "AccountId",
                principalTable: "UserAccounts",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricingRules_Courts_CourtId",
                table: "PricingRules",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "CourtId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
