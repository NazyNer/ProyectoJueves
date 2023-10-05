using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoJueves.Migrations
{
    public partial class TareaFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Título",
                table: "Tareas",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "Descripción",
                table: "Tareas",
                newName: "Descripcion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Tareas",
                newName: "Título");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Tareas",
                newName: "Descripción");
        }
    }
}
