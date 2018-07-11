using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WolfBack.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Players",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GameTypeId",
                table: "GameTypes",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Players",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GameTypes",
                newName: "GameTypeId");
        }
    }
}
