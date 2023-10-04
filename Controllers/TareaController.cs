using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
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
    return View();
  }

// No entiendo porque no anda... si me das una solucion profe podre seguir (se lo agradeceria mucho)...:(
  public JsonResult GuardarTarea(string Titulo,string Descripcion,DateTime FechaDeCarga, DateTime FechaDeVencimiento,int ProfesorID,int AsignaturaID)
  {
    dynamic Error = new ExpandoObject();
    Error.nonError = false;
    Error.mensaje = "hubo un error al crear la tarea";
    var Datos = new Tarea{
      FechaDeCarga = FechaDeCarga,
      FechaDeVencimiento = FechaDeVencimiento,
      Titulo = Titulo,
      Descripcion = Descripcion,
      AsignaturaID = AsignaturaID,
      ProfesorID = ProfesorID
    };
    Error.Dato = Datos;
    return Json(Error);
  }
}