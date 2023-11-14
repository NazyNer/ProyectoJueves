using System.ComponentModel.DataAnnotations;
using ProyectoJueves.Utils;

namespace ProyectoJueves.Models;

public class Asignatura {
    [Key]
    public int AsignaturaId { get; set; }
    public string? Nombre { get; set; }
    public int CarreraID  { get; set; }
    public string? CarreraNombre { get; set; }
    public Estado EstadoAsignatura  { get; set; }

}

