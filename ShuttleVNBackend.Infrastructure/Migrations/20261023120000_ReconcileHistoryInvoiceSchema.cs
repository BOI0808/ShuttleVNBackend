using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ShuttleVNBackend.Infrastructure.Persistence;

namespace ShuttleVNBackend.Infrastructure.Migrations;

[DbContext(typeof(ShuttleVnDbContext))]
[Migration("20261023120000_ReconcileHistoryInvoiceSchema")]
public sealed class ReconcileHistoryInvoiceSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            -- 1. Bổ sung CHECK constraints
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint WHERE conname = 'ck_booking_status_history_actor'
                ) THEN
                    ALTER TABLE "BookingStatusHistories"
                    ADD CONSTRAINT "ck_booking_status_history_actor"
                    CHECK (("ActorType" = 'System' AND "ChangedByEmployee" IS NULL AND "CustomerId" IS NULL) OR ("ActorType" = 'Employee' AND "ChangedByEmployee" IS NOT NULL AND "CustomerId" IS NULL) OR ("ActorType" = 'Customer' AND "CustomerId" IS NOT NULL AND "ChangedByEmployee" IS NULL));
                END IF;

                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint WHERE conname = 'ck_invoice_payment_group'
                ) THEN
                    ALTER TABLE "Invoices"
                    ADD CONSTRAINT "ck_invoice_payment_group"
                    CHECK (("Status" = 'Unpaid' AND "PaidAt" IS NULL AND "PaymentMethod" IS NULL AND "PaidByEmployee" IS NULL) OR ("Status" = 'Paid' AND "PaidAt" IS NOT NULL AND "PaymentMethod" IS NOT NULL AND "PaidByEmployee" IS NOT NULL) OR "Status" = 'Cancelled');
                END IF;

                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint WHERE conname = 'ck_invoice_daily_counter_positive'
                ) THEN
                    ALTER TABLE "InvoiceDailyCounters"
                    ADD CONSTRAINT "ck_invoice_daily_counter_positive"
                    CHECK ("LastNumber" > 0);
                END IF;
            END $$;

            -- 2. Bổ sung các chỉ mục hiệu năng & unique partial indexes
            CREATE INDEX IF NOT EXISTS "IX_Bookings_Status_CreatedAt_BookingId" 
                ON "Bookings" ("Status", "CreatedAt", "BookingId");

            CREATE INDEX IF NOT EXISTS "IX_Bookings_Date_CourtId_StartTime_BookingId" 
                ON "Bookings" ("Date", "CourtId", "StartTime", "BookingId");

            CREATE INDEX IF NOT EXISTS "IX_Customers_Phone" 
                ON "Customers" ("Phone");

            CREATE INDEX IF NOT EXISTS "IX_BookingStatusHistories_BookingId_ChangedAt_Id" 
                ON "BookingStatusHistories" ("BookingId", "ChangedAt", "Id");

            CREATE UNIQUE INDEX IF NOT EXISTS "IX_BookingStatusHistories_BookingId" 
                ON "BookingStatusHistories" ("BookingId") 
                WHERE "OldStatus" IS NULL;

            DROP INDEX IF EXISTS "IX_Invoices_BookingId";
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Invoices_BookingId" 
                ON "Invoices" ("BookingId") 
                WHERE "Status" IN ('Unpaid', 'Paid');

            -- 3. Chỉ mục hỗ trợ truy vấn lịch sử đặt sân của khách (S15-BE-6)
            CREATE INDEX IF NOT EXISTS "IX_Bookings_CustomerId_Date_StartTime_CreatedAt_BookingId" 
                ON "Bookings" ("CustomerId", "Date" DESC, "StartTime" DESC, "CreatedAt" DESC, "BookingId");
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP INDEX IF EXISTS "IX_Bookings_CustomerId_Date_StartTime_CreatedAt_BookingId";

            DROP INDEX IF EXISTS "IX_Invoices_BookingId";
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Invoices_BookingId" 
                ON "Invoices" ("BookingId") 
                WHERE "Status" = 'Unpaid';

            DROP INDEX IF EXISTS "IX_BookingStatusHistories_BookingId";
            DROP INDEX IF EXISTS "IX_BookingStatusHistories_BookingId_ChangedAt_Id";
            DROP INDEX IF EXISTS "IX_Customers_Phone";
            DROP INDEX IF EXISTS "IX_Bookings_Date_CourtId_StartTime_BookingId";
            DROP INDEX IF EXISTS "IX_Bookings_Status_CreatedAt_BookingId";

            ALTER TABLE "InvoiceDailyCounters" DROP CONSTRAINT IF EXISTS "ck_invoice_daily_counter_positive";
            ALTER TABLE "Invoices" DROP CONSTRAINT IF EXISTS "ck_invoice_payment_group";
            ALTER TABLE "BookingStatusHistories" DROP CONSTRAINT IF EXISTS "ck_booking_status_history_actor";
            """);
    }
}