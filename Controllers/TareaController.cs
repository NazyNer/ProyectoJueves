using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Migrations;
using ProyectoJueves.Models;
using RolName.Utils;

namespace TareaController.Controllers;

[Authorize(Roles = "Profesor,Estudiante")]
public class TareaController : Controller
{

  private readonly ILogger<TareaController> _logger;
  private readonly ApplicationDbContext _context;
  private readonly UserManager<IdentityUser> _userManager;

  public TareaController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<TareaController> logger)
  {
    _context = context;
    _logger = logger;
    _userManager = userManager;
  }
  public async Task<IActionResult> Index()
  {
    var user = await _userManager.GetUserAsync(User);
    RolNames rolName = new RolNames(_context);
    var rolRealName = rolName.RolNombre(user.Id);
    var asignaturas = _context.Asignaturas?.ToList();
    if (rolRealName == "Profesor")
    {
      var profesorID = _context.Profesores.Where(p => p.UsuarioID == user.Id).Select(p => p.ProfesorId).FirstOrDefault();
      var asignaturasAsignadas = _context.ProfesorAsignatura.Where(pa => pa.ProfesorID == profesorID).ToList();
      if (asignaturasAsignadas != null)
      {
        var Asignaturas = asignaturas;
        var count = 0;
        foreach (var asignatura in asignaturasAsignadas)
        {
          if (count == 0)
          {
            Asignaturas = asignaturas.Where(a => a.AsignaturaId == asignatura.AsignaturaID).ToList();
          }
          else
          {
            Asignaturas.Add(asignaturas.Where(a => a.AsignaturaId == asignatura.AsignaturaID).FirstOrDefault());
          }
          count++;
        }
        ViewBag.AsignaturaId = new SelectList(Asignaturas, "AsignaturaId", "Nombre");
      }
    }
    else if (rolRealName == "Estudiante")
    {
      var CarreraID = _context.Alumnos.Where(p => p.UsuarioID == user.Id).Select(p => p.CarreraId).FirstOrDefault();
      var Asignaturas = _context.Asignaturas.Where(pa => pa.CarreraID == CarreraID).ToList();
      ViewBag.AsignaturaId = new SelectList(Asignaturas, "AsignaturaId", "Nombre");
    }
    return View();
  }

  public JsonResult ObtenerDatos(int id, int asignaturaId)
  {
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

  [Authorize(Roles = "Profesor")]
  public JsonResult GuardarTarea(string titulo, string descripcion, DateTime FechaCarga, DateTime FechaVencimiento, int profesorID, int asignaturaID)
  {
    dynamic Error = new ExpandoObject();
    Error.nonError = false;
    Error.mensaje = "hubo un error al crear la tarea";
    var Tarea = new Tarea
    {
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