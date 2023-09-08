using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoJueves.Migrations
{
    public partial class ProfesorDni : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dni",
                table: "Profesores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Profesores");
        }
    }
}
