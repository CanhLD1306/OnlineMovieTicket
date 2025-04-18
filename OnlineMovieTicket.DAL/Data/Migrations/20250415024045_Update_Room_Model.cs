using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMovieTicket.DAL.Migrations
{
    public partial class Update_Room_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Column",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Row",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Column",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Row",
                table: "Rooms");
        }
    }
}
