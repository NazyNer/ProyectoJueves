using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoJueves.Data;
using ProyectoJueves.Models;

namespace ProyectoJueves.Controllers;

[Authorize]
public class CarreraController : Controller {
    private readonly ILogger<CarreraController> _logger;
    private readonly ApplicationDbContext _context;

    public CarreraController(ApplicationDbContext context, ILogger<CarreraController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }

    public JsonResult SearchCarrers(int Id){
        var carrers = _context.Carreras?.OrderBy(c => c.Name).ToList();
        if (Id > 0)
        {
            var carrer = _context.Carreras?.Where(c => c.Id == Id).FirstOrDefault() ;
            return Json(carrer);
        }
        return Json(carrers);
    }

    public JsonResult SaveCarrer(int Id, string Nombre, decimal Duracion){
        var error = new Errors();
        error.NonError = false;
        error.Msj = "No se pudo guardar la carrera";
        if (Id == 0)
        {
            var CarreraOriginal = _context.Carreras.Where(c => c.Name == Nombre).FirstOrDefault();
            if (CarreraOriginal != null)
            {
                error.NonError = false;
                error.Msj = "Ya existe una con ese nombre"; 
            }else
            {
                error.NonError = false;
                error.Msj = "la duracion no puede ser mas de 9 años"; 
                if (Duracion < 9)
                {
                    var Carrera = new Carrera(){
                    Name = Nombre,
                    Duration = Duracion,
                    };
                    _context.Add(Carrera);
                    _context.SaveChanges();
                    error.NonError = true;
                    error.Msj ="";
                }
                
            }
        }else{
            var CarreraOriginal = _context.Carreras.Where(c => c.Name == Nombre).FirstOrDefault();
            if (CarreraOriginal != null)
            {
                error.NonError = false;
                error.Msj = "Ya existe una con ese nombre"; 
            }else
            {
                error.NonError = false;
                error.Msj = "la duracion no puede ser mas de 9 años"; 
                if (Duracion < 10){
                    var Carrera = _context.Carreras?.Where(c => c.Id == Id).FirstOrDefault();
                    Carrera.Name = Nombre;
                    Carrera.Duration = Duracion;
                    _context.SaveChanges();
                    error.NonError = true;
                    error.Msj = "";
                }
            }
        }
        return Json(error);
    }
    public JsonResult DeleteCarrer(int Id){
        var error = new Errors();
        error.NonError = false;
        error.Msj = "No se selecciono niguna carrera";
        if (Id > 0)
        {
            error.NonError = false;
            error.Msj = "No se encuentra la carrera seleccionada";
            var carrer = _context.Carreras?.Where(c => c.Id == Id).FirstOrDefault();
            if (carrer != null)
            {
                error.NonError = false;
                error.Msj = "Se encuentran alumno relacionados a esta carrera";
                var AlumnosRelacionados = _context.Alumnos?.Where(a => a.CarreraId == Id).ToList();
                if (AlumnosRelacionados.Count == 0)
                {
                    _context.Remove(carrer);
                    _context.SaveChanges();
                    error.NonError = true;
                    error.Msj = "";
                    return Json(error);
                }
            }
        }
        return Json(error);
    }
}