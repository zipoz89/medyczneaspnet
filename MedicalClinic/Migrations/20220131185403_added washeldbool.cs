using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalClinic.Migrations
{
    public partial class addedwasheldbool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasHeld",
                schema: "Identity",
                table: "Appointment",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasHeld",
                schema: "Identity",
                table: "Appointment");
        }
    }
}
