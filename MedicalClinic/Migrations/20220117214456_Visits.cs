using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalClinic.Migrations
{
    public partial class Visits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visit",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatiendId = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    VisitTimeInMinutes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visit", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visit",
                schema: "Identity");
        }
    }
}
