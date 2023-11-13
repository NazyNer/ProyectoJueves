using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoJueves.Migrations
{
    public partial class ProfesYAlumnos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioID",
                table: "Profesores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioID",
                table: "Alumnos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Alumnos");
        }
    }
}
