using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoJueves.Migrations
{
    public partial class estadoTare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstadoCarrera",
                table: "Tareas",
                newName: "EstadoTarea");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstadoTarea",
                table: "Tareas",
                newName: "EstadoCarrera");
        }
    }
}
