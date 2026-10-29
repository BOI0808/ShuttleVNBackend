using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuttleVNBackend.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddReschedulingAndInvoiceLifecycle : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            LOCK TABLE "Invoices" IN ACCESS EXCLUSIVE MODE;

            DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM "Invoices" WHERE "RefundMethod" NOT IN (0, 1)) THEN
                    RAISE EXCEPTION 'Invoice migration aborted: unknown RefundMethod; manual repair required (Bank=0, Cash=1).';
                END IF;
                IF EXISTS (SELECT 1 FROM "Invoices" WHERE "CancellationActorType" NOT IN (0, 1, 2)) THEN
                    RAISE EXCEPTION 'Invoice migration aborted: unknown CancellationActorType; manual repair required (System=0, Employee=1, Customer=2).';
                END IF;
                IF EXISTS (
                    SELECT 1 FROM "Invoices"
                    WHERE "RefundAmount" IS NOT NULL
                      AND ("RefundAmount"::text IN ('NaN', 'Infinity', '-Infinity')
                           OR "RefundAmount" < -99999999.99 OR "RefundAmount" > 99999999.99
                           OR "RefundAmount" <> round("RefundAmount", 2))
                ) THEN
                    RAISE EXCEPTION 'Invoice migration aborted: RefundAmount cannot fit numeric(10,2) without loss; manual repair required.';
                END IF;
                IF EXISTS (
                    SELECT 1 FROM "Invoices" WHERE "ReplacesInvoiceId" IS NOT NULL
                    GROUP BY "ReplacesInvoiceId" HAVING count(*) > 1
                ) THEN
                    RAISE EXCEPTION 'Invoice migration aborted: duplicate ReplacesInvoiceId; manual repair required.';
                END IF;
            END $$;

            ALTER TABLE "Invoices" DROP CONSTRAINT "ck_invoice_payment_group";
            ALTER TABLE "Invoices" DROP CONSTRAINT "FK_Invoices_Employees_IssuedByEmployee";
            ALTER TABLE "Invoices" ALTER COLUMN "IssuedByEmployee" DROP NOT NULL;
            ALTER TABLE "Invoices" ALTER COLUMN "RefundAmount" TYPE numeric(10,2);
            ALTER TABLE "Invoices" ALTER COLUMN "RefundMethod" TYPE text
                USING CASE "RefundMethod" WHEN 0 THEN 'Bank' WHEN 1 THEN 'Cash' ELSE NULL END;
            ALTER TABLE "Invoices" ALTER COLUMN "CancellationActorType" TYPE text
                USING CASE "CancellationActorType" WHEN 0 THEN 'System' WHEN 1 THEN 'Employee' WHEN 2 THEN 'Customer' ELSE NULL END;

            CREATE INDEX "IX_Invoices_CancelledByEmployee" ON "Invoices" ("CancelledByEmployee");
            CREATE INDEX "IX_Invoices_CancelledByCustomer" ON "Invoices" ("CancelledByCustomer");
            CREATE INDEX "IX_Invoices_RefundedByEmployee" ON "Invoices" ("RefundedByEmployee");
            CREATE UNIQUE INDEX "IX_Invoices_ReplacesInvoiceId" ON "Invoices" ("ReplacesInvoiceId");

            ALTER TABLE "Invoices" ADD CONSTRAINT "FK_Invoices_Employees_IssuedByEmployee"
                FOREIGN KEY ("IssuedByEmployee") REFERENCES "Employees" ("EmployeeId") ON DELETE RESTRICT NOT VALID;
            ALTER TABLE "Invoices" ADD CONSTRAINT "FK_Invoices_Employees_CancelledByEmployee"
                FOREIGN KEY ("CancelledByEmployee") REFERENCES "Employees" ("EmployeeId") ON DELETE RESTRICT NOT VALID;
            ALTER TABLE "Invoices" ADD CONSTRAINT "FK_Invoices_Customers_CancelledByCustomer"
                FOREIGN KEY ("CancelledByCustomer") REFERENCES "Customers" ("CustomerId") ON DELETE RESTRICT NOT VALID;
            ALTER TABLE "Invoices" ADD CONSTRAINT "FK_Invoices_Employees_RefundedByEmployee"
                FOREIGN KEY ("RefundedByEmployee") REFERENCES "Employees" ("EmployeeId") ON DELETE RESTRICT NOT VALID;
            ALTER TABLE "Invoices" ADD CONSTRAINT "FK_Invoices_Invoices_ReplacesInvoiceId"
                FOREIGN KEY ("ReplacesInvoiceId") REFERENCES "Invoices" ("InvoiceId") ON DELETE RESTRICT NOT VALID;

            ALTER TABLE "Invoices" ADD CONSTRAINT "ck_invoice_payment_group" CHECK (("Status" IN ('Unpaid','Cancelled') AND "PaidAt" IS NULL AND "PaymentMethod" IS NULL AND "PaidByEmployee" IS NULL) OR ("Status" IN ('Paid','Refunded') AND "PaidAt" IS NOT NULL AND "PaymentMethod" IS NOT NULL AND "PaidByEmployee" IS NOT NULL)) NOT VALID;
            ALTER TABLE "Invoices" ADD CONSTRAINT "ck_invoice_refund_group" CHECK (("Status" = 'Refunded' AND "RefundedByEmployee" IS NOT NULL AND "RefundedAt" IS NOT NULL AND "RefundMethod" IS NOT NULL AND "RefundAmount" IS NOT NULL AND "RefundAmount" = "TotalCost" AND "RefundAmount" > 0 AND "RefundReason" IS NOT NULL AND length(btrim("RefundReason")) > 0 AND "RefundedAt" >= "PaidAt") OR ("Status" <> 'Refunded' AND "RefundedByEmployee" IS NULL AND "RefundedAt" IS NULL AND "RefundMethod" IS NULL AND "RefundAmount" IS NULL AND "RefundReason" IS NULL)) NOT VALID;
            ALTER TABLE "Invoices" ADD CONSTRAINT "ck_invoice_cancellation_group" CHECK (("Status" IN ('Unpaid','Paid') AND "CancelledAt" IS NULL AND "CancellationActorType" IS NULL AND "CancelledByEmployee" IS NULL AND "CancelledByCustomer" IS NULL AND "CancellationReason" IS NULL) OR ("Status" IN ('Cancelled','Refunded') AND "CancelledAt" IS NOT NULL AND "CancellationActorType" IS NOT NULL AND "CancellationReason" IS NOT NULL AND length(btrim("CancellationReason")) > 0 AND (("CancellationActorType" = 'System' AND "CancelledByEmployee" IS NULL AND "CancelledByCustomer" IS NULL) OR ("CancellationActorType" = 'Customer' AND "CancelledByCustomer" IS NOT NULL AND "CancelledByEmployee" IS NULL) OR ("CancellationActorType" = 'Employee' AND "CancelledByEmployee" IS NOT NULL AND "CancelledByCustomer" IS NULL)))) NOT VALID;

            DO $$
            DECLARE
                constraint_name text;
            BEGIN
                FOREACH constraint_name IN ARRAY ARRAY[
                    'FK_Invoices_Employees_IssuedByEmployee',
                    'FK_Invoices_Employees_CancelledByEmployee',
                    'FK_Invoices_Customers_CancelledByCustomer',
                    'FK_Invoices_Employees_RefundedByEmployee',
                    'FK_Invoices_Invoices_ReplacesInvoiceId',
                    'ck_invoice_payment_group',
                    'ck_invoice_refund_group',
                    'ck_invoice_cancellation_group'
                ] LOOP
                    BEGIN
                        EXECUTE format('ALTER TABLE "Invoices" VALIDATE CONSTRAINT %I', constraint_name);
                    EXCEPTION WHEN check_violation OR foreign_key_violation THEN
                        RAISE EXCEPTION 'Invoice migration aborted: incompatible data for constraint %; manual repair required.', constraint_name;
                    END;
                END LOOP;
            END $$;
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            LOCK TABLE "Invoices" IN ACCESS EXCLUSIVE MODE;

            DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM "Invoices" WHERE "Status" = 'Refunded') THEN
                    RAISE EXCEPTION 'Invoice rollback aborted: Refunded invoices are incompatible with the baseline; manual repair required.';
                END IF;
                IF EXISTS (SELECT 1 FROM "Invoices" WHERE "IssuedByEmployee" IS NULL) THEN
                    RAISE EXCEPTION 'Invoice rollback aborted: null IssuedByEmployee is incompatible with the baseline; manual repair required.';
                END IF;
                IF EXISTS (SELECT 1 FROM "Invoices" WHERE "RefundMethod" NOT IN ('Bank', 'Cash')) THEN
                    RAISE EXCEPTION 'Invoice rollback aborted: unknown RefundMethod; manual repair required (Bank, Cash).';
                END IF;
                IF EXISTS (SELECT 1 FROM "Invoices" WHERE "CancellationActorType" NOT IN ('System', 'Employee', 'Customer')) THEN
                    RAISE EXCEPTION 'Invoice rollback aborted: unknown CancellationActorType; manual repair required (System, Employee, Customer).';
                END IF;
            END $$;

            ALTER TABLE "Invoices" DROP CONSTRAINT "ck_invoice_cancellation_group";
            ALTER TABLE "Invoices" DROP CONSTRAINT "ck_invoice_refund_group";
            ALTER TABLE "Invoices" DROP CONSTRAINT "ck_invoice_payment_group";
            ALTER TABLE "Invoices" DROP CONSTRAINT "FK_Invoices_Invoices_ReplacesInvoiceId";
            ALTER TABLE "Invoices" DROP CONSTRAINT "FK_Invoices_Employees_RefundedByEmployee";
            ALTER TABLE "Invoices" DROP CONSTRAINT "FK_Invoices_Customers_CancelledByCustomer";
            ALTER TABLE "Invoices" DROP CONSTRAINT "FK_Invoices_Employees_CancelledByEmployee";
            ALTER TABLE "Invoices" DROP CONSTRAINT "FK_Invoices_Employees_IssuedByEmployee";

            DROP INDEX "IX_Invoices_ReplacesInvoiceId";
            DROP INDEX "IX_Invoices_RefundedByEmployee";
            DROP INDEX "IX_Invoices_CancelledByCustomer";
            DROP INDEX "IX_Invoices_CancelledByEmployee";

            ALTER TABLE "Invoices" ALTER COLUMN "RefundMethod" TYPE integer
                USING CASE "RefundMethod" WHEN 'Bank' THEN 0 WHEN 'Cash' THEN 1 ELSE NULL END;
            ALTER TABLE "Invoices" ALTER COLUMN "CancellationActorType" TYPE integer
                USING CASE "CancellationActorType" WHEN 'System' THEN 0 WHEN 'Employee' THEN 1 WHEN 'Customer' THEN 2 ELSE NULL END;
            ALTER TABLE "Invoices" ALTER COLUMN "RefundAmount" TYPE numeric;
            ALTER TABLE "Invoices" ALTER COLUMN "IssuedByEmployee" SET NOT NULL;
            ALTER TABLE "Invoices" ADD CONSTRAINT "FK_Invoices_Employees_IssuedByEmployee"
                FOREIGN KEY ("IssuedByEmployee") REFERENCES "Employees" ("EmployeeId") ON DELETE CASCADE;
            ALTER TABLE "Invoices" ADD CONSTRAINT "ck_invoice_payment_group" CHECK (("Status" = 'Unpaid' AND "PaidAt" IS NULL AND "PaymentMethod" IS NULL AND "PaidByEmployee" IS NULL) OR ("Status" = 'Paid' AND "PaidAt" IS NOT NULL AND "PaymentMethod" IS NOT NULL AND "PaidByEmployee" IS NOT NULL) OR "Status" = 'Cancelled') NOT VALID;

            DO $$
            BEGIN
                ALTER TABLE "Invoices" VALIDATE CONSTRAINT "ck_invoice_payment_group";
            EXCEPTION WHEN check_violation THEN
                RAISE EXCEPTION 'Invoice rollback aborted: incompatible data for baseline ck_invoice_payment_group; manual repair required.';
            END $$;
            """);
    }
}

