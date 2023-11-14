using System.ComponentModel.DataAnnotations;
using ProyectoJueves.Utils;

namespace ProyectoJueves.Models
{
    public class Tarea
    {
        [Key]
        public int TareaId { get; set; }
        public DateTime FechaDeCarga { get; set; }
        public DateTime FechaDeVencimiento { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public int AsignaturaID { get; set; }
        public int ProfesorID { get; set; }
        public Estado EstadoTarea { get; set; }
    }
}