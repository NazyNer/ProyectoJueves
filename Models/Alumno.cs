using System.ComponentModel.DataAnnotations;

namespace ProyectoJueves.Models;

public class Alumno {
    [Key]
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public int DNI { get; set; }
    public DateTime Birthdate { get; set; }
    public string? Email { get; set; }
    public int CarreraId { get; set; }
    public string? CarreraName { get; set;}
    public bool IsActive { get; set; }
    public virtual Carrera? Carrera { get; set; }
}