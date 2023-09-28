using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoJueves.Models;

namespace ProyectoJueves.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Carrera> Carreras { get; set; }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Profesor> Profesores { get; set; }
    public DbSet<Asignatura> Asignaturas { get; set; }
    public DbSet<ProfesorAsignatura> ProfesorAsignatura { get; set; }
}
