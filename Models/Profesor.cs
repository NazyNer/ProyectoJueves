using System.ComponentModel.DataAnnotations;
using ProyectoJueves.Utils;

namespace ProyectoJueves.Models;

public class Profesor{
    [Key]
    public int ProfesorId { get; set; }
    public string? FullName { get; set; }
    public DateTime Birthdate { get; set; }
    public string? Address { get; set; }
    public int Dni { get; set; }
    public string? Email { get; set; }
    public string? UsuarioID { get; set; }
    public Estado EstadoProfesor { get; set; }
}