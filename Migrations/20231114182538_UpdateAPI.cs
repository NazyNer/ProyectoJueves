using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoJueves.Migrations
{
    public partial class UpdateAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alumnos_Carreras_CarreraId",
                table: "Alumnos");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_CarreraId",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Alumnos");

            migrationBuilder.AddColumn<int>(
                name: "EstadoCarrera",
                table: "Tareas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoProfesor",
                table: "Profesores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoCarrera",
                table: "Carreras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoAsignatura",
                table: "Asignaturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoAlumno",
                table: "Alumnos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoCarrera",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "EstadoProfesor",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "EstadoCarrera",
                table: "Carreras");

            migrationBuilder.DropColumn(
                name: "EstadoAsignatura",
                table: "Asignaturas");

            migrationBuilder.DropColumn(
                name: "EstadoAlumno",
                table: "Alumnos");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Alumnos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_CarreraId",
                table: "Alumnos",
                column: "CarreraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alumnos_Carreras_CarreraId",
                table: "Alumnos",
                column: "CarreraId",
                principalTable: "Carreras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
