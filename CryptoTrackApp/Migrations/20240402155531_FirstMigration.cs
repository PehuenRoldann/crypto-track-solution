using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrackApp.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    birth_date = table.Column<DateTime>(type: "date", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.UniqueConstraint("ak_users_email", x => x.email);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_id = table.Column<string>(type: "text", nullable: false),
                    notification_umbral = table.Column<double>(type: "double precision", nullable: false),
                    follow_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    unfollow_date = table.Column<DateTime>(type: "timestamp with time 	zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.subscription_id);
                    table.ForeignKey(
                        name: "fk_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "birth_date", "email", "password", "status", "user_name" },
                values: new object[] { new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6"), new DateTime(2000, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "pehuen@gmail.com", "Password123!", true, "Pehuén" });

            migrationBuilder.InsertData(
                table: "subscriptions",
                columns: new[] { "subscription_id", "currency_id", "follow_date", "notification_umbral", "unfollow_date", "user_id" },
                values: new object[,]
                {
                    { new Guid("02a521e0-8625-4fe4-8425-51a36e753c6b"), "bitcoin", new DateTime(2024, 4, 2, 15, 55, 30, 770, DateTimeKind.Utc).AddTicks(1810), 0.5, null, new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6") },
                    { new Guid("7fefd8e7-65c4-4457-9cd1-b58acd83f36b"), "solana", new DateTime(2024, 4, 2, 15, 55, 30, 770, DateTimeKind.Utc).AddTicks(1818), 0.5, null, new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6") },
                    { new Guid("f8d07281-b9bd-4137-9f42-a6faecbba632"), "dia", new DateTime(2024, 4, 2, 15, 55, 30, 770, DateTimeKind.Utc).AddTicks(1814), 0.5, null, new Guid("7a43853d-b414-4432-b00c-5fd18f77abf6") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
