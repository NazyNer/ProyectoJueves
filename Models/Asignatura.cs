using System.ComponentModel.DataAnnotations;

namespace ProyectoJueves.Models;

public class Asignatura {
    [Key]
    public int AsignaturaId { get; set; }
    public string? Nombre { get; set; }
    public int CarreraID  { get; set; }
    public string? CarreraNombre { get; set; }
}