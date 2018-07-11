using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WolfBack.Migrations
{
    public partial class Scores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GameTypes",
                newName: "GameTypeId");

            migrationBuilder.AddColumn<Guid>(
                name: "ScoreId",
                table: "GameTypes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreId",
                table: "GameTypes");

            migrationBuilder.RenameColumn(
                name: "GameTypeId",
                table: "GameTypes",
                newName: "Id");
        }
    }
}
