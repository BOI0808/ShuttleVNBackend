using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullEmployeeAccountAndEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

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

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees",
                column: "AccountId",
                principalTable: "UserAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

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

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_UserAccounts_AccountId",
                table: "Employees",
                column: "AccountId",
                principalTable: "UserAccounts",
                principalColumn: "AccountId");
        }
    }
}
