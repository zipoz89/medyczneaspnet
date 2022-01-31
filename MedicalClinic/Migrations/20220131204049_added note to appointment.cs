using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalClinic.Migrations
{
    public partial class addednotetoappointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                schema: "Identity",
                table: "Appointment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                schema: "Identity",
                table: "Appointment");
        }
    }
}
