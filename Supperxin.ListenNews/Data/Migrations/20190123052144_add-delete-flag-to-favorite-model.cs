using Microsoft.EntityFrameworkCore.Migrations;

namespace Supperxin.ListenNews.Migrations
{
    public partial class adddeleteflagtofavoritemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Favorite",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Favorite");
        }
    }
}
