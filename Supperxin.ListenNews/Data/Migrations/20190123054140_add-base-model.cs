using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Supperxin.ListenNews.Migrations
{
    public partial class addbasemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Favorite",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Favorite",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Favorite");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Favorite");
        }
    }
}
