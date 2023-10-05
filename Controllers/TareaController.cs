using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Migrations;
using ProyectoJueves.Models;


namespace TareaController.Controllers;
[Authorize]
public class TareaController : Controller
{

  private readonly ILogger<TareaController> _logger;
  private readonly ApplicationDbContext _context;

  public TareaController(ApplicationDbContext context, ILogger<TareaController> logger)
  {
    _context = context;
    _logger = logger;
  }
  public IActionResult Index()
  {
    var asignaturas = _context.Asignaturas?.ToList();
    ViewBag.AsignaturaId = new SelectList(asignaturas?.OrderBy(p => p.Nombre), "AsignaturaId", "Nombre");
    return View();
  }
  public JsonResult ObtenerDatos(int id, int asignaturaId){
    dynamic Error = new ExpandoObject();
    Error.nonError = false;
    Error.mensaje = "No hay tareas en esta asignatura";
    var tareas = _context.Tareas?.Where(t => t.AsignaturaID == asignaturaId).OrderByDescending(t => t.FechaDeVencimiento).ToList();
    if (tareas.Count != 0)
    {
      if (id != 0)
      {
        tareas = tareas.Where(t => t.TareaId == id).ToList();
      }
      Error.nonError = true;
      Error.mensaje = tareas;
    }
    return Json(Error);
  }

  public JsonResult GuardarTarea(string titulo,string descripcion,DateTime FechaCarga, DateTime FechaVencimiento,int profesorID,int asignaturaID)
  {
    dynamic Error = new ExpandoObject();
    Error.nonError = false;
    Error.mensaje = "hubo un error al crear la tarea";
    var Tarea = new Tarea{
      FechaDeCarga = FechaCarga,
      FechaDeVencimiento = FechaVencimiento,
      Titulo = titulo,
      Descripcion = descripcion,
      AsignaturaID = asignaturaID,
      ProfesorID = profesorID
    };
    _context.Tareas.Add(Tarea);
    _context.SaveChanges();
    Error.nonError = true;
    Error.mensaje = "";
    return Json(Error);
  }

}