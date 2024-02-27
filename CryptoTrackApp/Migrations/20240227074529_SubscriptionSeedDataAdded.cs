using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrackApp.Migrations
{
    public partial class SubscriptionSeedDataAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "unfollow_date",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "follow_date",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.InsertData(
                table: "subscriptions",
                columns: new[] { "subscription_id", "currency_id", "follow_date", "notification_umbral", "unfollow_date", "user_id" },
                values: new object[,]
                {
                    { new Guid("02a521e0-8625-4fe4-8425-51a36e753c6b"), "bitcoin", new DateTime(2024, 2, 27, 7, 45, 28, 749, DateTimeKind.Utc).AddTicks(9638), 0.5, null, new Guid("4d266202-d63e-4caf-a87f-6ef56e0dd1b6") },
                    { new Guid("7fefd8e7-65c4-4457-9cd1-b58acd83f36b"), "solana", new DateTime(2024, 2, 27, 7, 45, 28, 749, DateTimeKind.Utc).AddTicks(9645), 0.5, null, new Guid("4d266202-d63e-4caf-a87f-6ef56e0dd1b6") },
                    { new Guid("f8d07281-b9bd-4137-9f42-a6faecbba632"), "dia", new DateTime(2024, 2, 27, 7, 45, 28, 749, DateTimeKind.Utc).AddTicks(9642), 0.5, null, new Guid("4d266202-d63e-4caf-a87f-6ef56e0dd1b6") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "subscriptions",
                keyColumn: "subscription_id",
                keyValue: new Guid("02a521e0-8625-4fe4-8425-51a36e753c6b"));

            migrationBuilder.DeleteData(
                table: "subscriptions",
                keyColumn: "subscription_id",
                keyValue: new Guid("7fefd8e7-65c4-4457-9cd1-b58acd83f36b"));

            migrationBuilder.DeleteData(
                table: "subscriptions",
                keyColumn: "subscription_id",
                keyValue: new Guid("f8d07281-b9bd-4137-9f42-a6faecbba632"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "unfollow_date",
                table: "subscriptions",
                type: "timestamp",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "follow_date",
                table: "subscriptions",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
