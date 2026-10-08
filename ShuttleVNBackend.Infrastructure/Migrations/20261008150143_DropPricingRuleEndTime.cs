using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropPricingRuleEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"PricingRules\" DROP CONSTRAINT IF EXISTS \"ex_pricing_rules_no_overlap\";");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PricingRules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "PricingRules",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0));

            migrationBuilder.Sql("ALTER TABLE \"PricingRules\" ADD CONSTRAINT \"ex_pricing_rules_no_overlap\" EXCLUDE USING gist (\"CourtId\" WITH =, \"DayOfWeek\" WITH =, (tsrange(DATE '2000-01-01' + \"StartTime\", DATE '2000-01-01' + \"EndTime\")) WITH &&);");
        }
    }
}
