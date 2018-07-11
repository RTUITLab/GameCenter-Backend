using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WolfBack.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_GameTypes_GameTypeId",
                table: "Scores");

            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Players_PlayerNamePlayerId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_PlayerNamePlayerId",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "PlayerNamePlayerId",
                table: "Scores");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameTypeId",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerId",
                table: "Scores",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Scores_PlayerId",
                table: "Scores",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_GameTypes_GameTypeId",
                table: "Scores",
                column: "GameTypeId",
                principalTable: "GameTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Players_PlayerId",
                table: "Scores",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_GameTypes_GameTypeId",
                table: "Scores");

            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Players_PlayerId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_PlayerId",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Scores");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameTypeId",
                table: "Scores",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerNamePlayerId",
                table: "Scores",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_PlayerNamePlayerId",
                table: "Scores",
                column: "PlayerNamePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_GameTypes_GameTypeId",
                table: "Scores",
                column: "GameTypeId",
                principalTable: "GameTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Players_PlayerNamePlayerId",
                table: "Scores",
                column: "PlayerNamePlayerId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
