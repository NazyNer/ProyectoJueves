using System.ComponentModel.DataAnnotations;

namespace ProyectoJueves.Models;

public class ProfesorAsignatura{
    [Key]
    public int ID { get; set; }
    public int ProfesorID { get; set; }
    public int AsignaturaID { get; set; }
}