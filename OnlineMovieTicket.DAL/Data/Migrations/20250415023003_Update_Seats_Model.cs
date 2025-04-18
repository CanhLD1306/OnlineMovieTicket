using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMovieTicket.DAL.Migrations
{
    public partial class Update_Seats_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Seats");

            migrationBuilder.AddColumn<int>(
                name: "ColumnIndex",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowIndex",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnIndex",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "RowIndex",
                table: "Seats");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Seats",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
