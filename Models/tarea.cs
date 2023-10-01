using System.ComponentModel.DataAnnotations;
using ProyectoJueves.Migrations;

public class Tarea
{
    [Key]
    public int TareaId { get; set; }
    public DateTime FechaDeCarga { get; set; }
    public DateTime FechaDeVencimiento { get; set; }
    public string? Título { get; set; }
    public string? Descripción { get; set; }
    public int AsignaturaID { get; set; }
    public int ProfesorID { get; set; }
}