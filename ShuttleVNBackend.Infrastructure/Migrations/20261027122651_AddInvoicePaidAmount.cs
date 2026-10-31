using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoicePaidAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                LOCK TABLE "Invoices" IN ACCESS EXCLUSIVE MODE;

                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM "Invoices"
                        WHERE "Status" IN ('Paid', 'Refunded')
                          AND ("TotalCost" <= 0 OR "TotalCost" <> trunc("TotalCost")
                               OR "TotalCost"::text IN ('NaN', 'Infinity', '-Infinity'))
                    ) THEN
                        RAISE EXCEPTION 'Paid amount migration aborted: paid/refunded total must be positive whole VND; manual repair required.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Invoices",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            // Approved legacy backfill; existing payment actors, times and methods stay unchanged.
            migrationBuilder.Sql("""
                UPDATE "Invoices" SET "PaidAmount" = "TotalCost"
                WHERE "Status" IN ('Paid', 'Refunded');
                """);

            migrationBuilder.AddCheckConstraint(
                name: "ck_invoice_paid_amount",
                table: "Invoices",
                sql: "(\"Status\" IN ('Unpaid','Cancelled') AND \"PaidAmount\" IS NULL) OR (\"Status\" IN ('Paid','Refunded') AND \"PaidAmount\" IS NOT NULL AND \"PaidAmount\" > 0 AND \"PaidAmount\" = trunc(\"PaidAmount\") AND \"PaidAmount\" = \"TotalCost\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_invoice_paid_amount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Invoices");
        }
    }
}
