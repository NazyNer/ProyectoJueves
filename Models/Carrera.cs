using System.ComponentModel.DataAnnotations;

namespace ProyectoJueves.Models;

public class Carrera {
    [Key]
    public int Id { get; set;}
    public string? Name { get; set;}
    public decimal Duration { get; set;}
}