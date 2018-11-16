using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WolfBack.Migrations
{
    public partial class date1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Scores");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Scores",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Scores");

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "Scores",
                nullable: true);
        }
    }
}
