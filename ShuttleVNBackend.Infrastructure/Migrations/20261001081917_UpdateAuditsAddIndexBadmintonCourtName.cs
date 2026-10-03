using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditsAddIndexBadmintonCourtName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_UserAccounts_AccountId",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Audits",
                newName: "ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_AccountId",
                table: "Audits",
                newName: "IX_Audits_ActorId");

            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "Audits",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "Audits",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                table: "Audits",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "Audits",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Audits",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ActorType",
                table: "Audits",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BadmintonCourts_Name",
                table: "BadmintonCourts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CreatedAt",
                table: "Audits",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_EntityName_EntityId",
                table: "Audits",
                columns: new[] { "EntityName", "EntityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BadmintonCourts_Name",
                table: "BadmintonCourts");

            migrationBuilder.DropIndex(
                name: "IX_Audits_CreatedAt",
                table: "Audits");

            migrationBuilder.DropIndex(
                name: "IX_Audits_EntityName_EntityId",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "ActorType",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                table: "Audits",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_ActorId",
                table: "Audits",
                newName: "IX_Audits_AccountId");

            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "Audits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "Audits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                table: "Audits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "Audits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Audits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_UserAccounts_AccountId",
                table: "Audits",
                column: "AccountId",
                principalTable: "UserAccounts",
                principalColumn: "AccountId");
        }
    }
}
