using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Models;
using ProyectoJueves.Utils;

namespace ProyectoJueves.Controllers;

[Authorize(Roles = "Admin")]
public class AsignaturaController : Controller {
    private readonly ILogger<AsignaturaController> _logger;
    private readonly ApplicationDbContext _context;

    public AsignaturaController(ApplicationDbContext context, ILogger<AsignaturaController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        var Carreras = _context.Carreras.Where(c => c.EstadoCarrera != Estado.Desactivado).ToList();
        ViewBag.CarreraID = new SelectList(Carreras?.OrderBy(p => p.Name), "Id", "Name");
        return View();
    }
    public JsonResult BuscarAsignaturas(int id){
      var Asignaturas = _context.Asignaturas.Where(a => a.EstadoAsignatura != Estado.Desactivado).ToList();
      Asignaturas = Asignaturas.OrderBy(a => a.Nombre).ToList();
      if(id > 0){
        Asignaturas = Asignaturas.Where(a => a.AsignaturaId == id).ToList();
      }
      return Json(Asignaturas);
    }
    public JsonResult Guardar( int Id, string Nombre, int CarreraId){
      var error = new Errors();
      error.NonError = false;
      error.Msj = "No se pudo guardar la asignatura";
      var carrera = _context.Carreras.Where(c => c.Id == CarreraId).FirstOrDefault();
      if (carrera != null){
        if (Id == 0)
        {
            var Asignatura = new Asignatura{
              Nombre = Nombre,
              CarreraID = CarreraId,
              CarreraNombre = carrera.Name,
              EstadoAsignatura = Estado.Activo
            };
            _context.Asignaturas.Add(Asignatura);
            _context.SaveChanges();
            error.NonError = true;
        }else{
          var asignatura = _context.Asignaturas.Where(a => a.AsignaturaId == Id).FirstOrDefault();
          if (asignatura != null)
          {
            asignatura.Nombre = Nombre;
            asignatura.CarreraID = CarreraId;
            asignatura.CarreraNombre = carrera.Name;
            asignatura.EstadoAsignatura = Estado.Activo;
            _context.SaveChanges();
            error.NonError = true;
          }else{
            error.Msj = "La asignatura seleccionada no se encuentra";
          }
        }
      }else{
        error.Msj = "La carrera seleccionada no se encuentra";
      }
      return Json(error);
    }

    public JsonResult Eliminar ( int Id = 0){
      var error = new Errors();
      error.NonError = false;
      error.Msj = "No se pudo eliminar la asignatura";
      if (Id > 0){
        var Asignatura = _context.Asignaturas.Where(a => a.AsignaturaId == Id).FirstOrDefault();
        if (Asignatura != null)
        {
          Asignatura.EstadoAsignatura = Estado.Eliminado;
          _context.SaveChanges();
          error.NonError = true;
          error.Msj = "";
        }
      }
      return Json(error);
    }
}