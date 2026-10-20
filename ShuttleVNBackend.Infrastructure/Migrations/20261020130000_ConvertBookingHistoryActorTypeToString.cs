using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ShuttleVNBackend.Infrastructure.Persistence;

namespace ShuttleVNBackend.Infrastructure.Migrations;

[DbContext(typeof(ShuttleVnDbContext))]
[Migration("20261020130000_ConvertBookingHistoryActorTypeToString")]
public sealed class ConvertBookingHistoryActorTypeToString : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM "BookingStatusHistories"
                    WHERE "ActorType"::text NOT IN ('0', '1', '2', 'System', 'Employee', 'Customer')
                ) THEN
                    RAISE EXCEPTION 'Cannot convert unknown BookingStatusHistories.ActorType values.';
                END IF;
            END $$;

            ALTER TABLE "BookingStatusHistories" ALTER COLUMN "ActorType" DROP DEFAULT;
            ALTER TABLE "BookingStatusHistories"
                ALTER COLUMN "ActorType" TYPE character varying(20)
                USING (CASE "ActorType"::text
                    WHEN '0' THEN 'System'
                    WHEN '1' THEN 'Employee'
                    WHEN '2' THEN 'Customer'
                    ELSE "ActorType"::text
                END);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM "BookingStatusHistories"
                    WHERE "ActorType" NOT IN ('System', 'Employee', 'Customer')
                ) THEN
                    RAISE EXCEPTION 'Cannot roll back unknown BookingStatusHistories.ActorType values.';
                END IF;
            END $$;

            ALTER TABLE "BookingStatusHistories" ALTER COLUMN "ActorType" DROP DEFAULT;
            ALTER TABLE "BookingStatusHistories"
                ALTER COLUMN "ActorType" TYPE integer
                USING (CASE "ActorType"
                    WHEN 'System' THEN 0
                    WHEN 'Employee' THEN 1
                    WHEN 'Customer' THEN 2
                END);
            ALTER TABLE "BookingStatusHistories" ALTER COLUMN "ActorType" SET DEFAULT 0;
            """);
    }
}
