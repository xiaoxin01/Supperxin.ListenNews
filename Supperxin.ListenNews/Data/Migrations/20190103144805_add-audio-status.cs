using Microsoft.EntityFrameworkCore.Migrations;

namespace Supperxin.ListenNews.Migrations
{
    public partial class addaudiostatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioErrorMessage",
                table: "Item",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AudioStatus",
                table: "Item",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioErrorMessage",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "AudioStatus",
                table: "Item");
        }
    }
}
