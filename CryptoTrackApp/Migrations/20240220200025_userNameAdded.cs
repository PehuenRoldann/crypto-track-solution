using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrackApp.Migrations
{
    public partial class userNameAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_name",
                table: "users");
        }
    }
}
