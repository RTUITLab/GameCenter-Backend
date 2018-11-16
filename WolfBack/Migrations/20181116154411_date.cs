using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WolfBack.Migrations
{
    public partial class date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "Scores",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Time",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
