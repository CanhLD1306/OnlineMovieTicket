using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMovieTicket.DAL.Migrations
{
    public partial class Update_Seat_ShowtimeSeat_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatTypeId",
                table: "ShowtimeSeats");

            migrationBuilder.AddColumn<long>(
                name: "SeatTypeId",
                table: "Seats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatTypeId",
                table: "Seats");

            migrationBuilder.AddColumn<long>(
                name: "SeatTypeId",
                table: "ShowtimeSeats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
