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
        var carrers = _context.Carreras?.ToList();
        if (Id > 0)
        {
            carrers?.Where(c => c.Id == Id).ToList();
        }
        carrers?.OrderBy(c => c.Name).ToList();
        return Json(carrers);
    }

    public JsonResult SaveCarrer(int Id, string Nombre, decimal Duracion){
        var error = new Errors();
        error.NonError = false;
        error.Msj = "No se pudo gurdar la carrera";
        if (Id == 0)
        {
            var CarreraOriginal = _context.Carreras.Where(c => c.Name == Nombre).FirstOrDefault();
            if (CarreraOriginal != null)
            {
                error.NonError = false;
                error.Msj = "Ya existe una con ese nombre"; 
            }else
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
        return Json(error);
    }
}