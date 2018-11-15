using Microsoft.EntityFrameworkCore.Migrations;

namespace WolfBack.Migrations
{
    public partial class imge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "GameTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "GameTypes");
        }
    }
}
