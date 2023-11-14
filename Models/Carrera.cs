using System.ComponentModel.DataAnnotations;
using ProyectoJueves.Utils;

namespace ProyectoJueves.Models;

public class Carrera {
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Duration { get; set; }
    public Estado EstadoCarrera { get; set; }
}