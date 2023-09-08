using System.Diagnostics;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Models;


namespace ProyectoJueves.Controllers;


[Authorize]
public class AlumnoController : Controller {

    private readonly ILogger<AlumnoController> _logger;
    private readonly ApplicationDbContext _context;

    public AlumnoController(ApplicationDbContext context, ILogger<AlumnoController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        var Carrera = _context.Carreras?.ToList();
        ViewBag.CarreraID = new SelectList(Carrera?.OrderBy(p => p.Name), "Id", "Name");
        return View();
    }

    public JsonResult SearchStudents(int Id)
    {
        var alumnos = _context.Alumnos.ToList();
        alumnos = alumnos.OrderBy(a =>a.CarreraName).ThenBy(a => a.FullName).ToList();
        if(Id > 0){
            alumnos = alumnos.Where(a => a.Id == Id).ToList();
        }
        return Json(alumnos);
    }

    public JsonResult GuardarAlumno(int Id, string FullName, DateTime Birthdate, int CarreraId)
    {
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "Completar datos obligatorios";
        // validacion de que todos los datos obligatorios sean ingresados
        if (!string.IsNullOrEmpty(FullName) && CarreraId > 0)
        {
            Error.NonError = false;
            Error.Mensaje = "Carrera no encontrada";
            var carreraOriginal = _context.Carreras?.Where(c => c.Id == CarreraId).FirstOrDefault();
            if (carreraOriginal != null)
            {
                Error.NonError = true;
                Error.Mensaje = "Datos guardados correctamente";
                if (Id == 0)
                {
                    //Crear Alumno
                    var Alumno = new Alumno{
                    FullName = FullName,
                    Birthdate = Birthdate,
                    CarreraId = carreraOriginal.Id,
                    CarreraName = carreraOriginal.Name,
                    IsActive = true
                    };
                    _context.Alumnos.Add(Alumno);
                    _context.SaveChanges();
                }else{
                    //Editar Alumno
                    var Alumno = _context.Alumnos.Where(a => a.Id == Id).FirstOrDefault();
                    if (Alumno != null)
                    {
                        Alumno.FullName = FullName;
                        Alumno.Birthdate = Birthdate;
                        Alumno.CarreraId = carreraOriginal.Id;
                        Alumno.CarreraName = carreraOriginal.Name;
                        _context.SaveChanges();
                    }
                }
            }
        }  
        return Json(Error);
    }

    public JsonResult DeleteAlumno(int id){
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "No se encontro el alumno seleccionado";
        var Alumno = _context.Alumnos.Where(a => a.Id == id).FirstOrDefault();
        if (Alumno != null)
        {
            Error.NonError = false;
            Error.Mensaje = "Primero desactivar al alumno "+ Alumno.FullName +" y despues volver a intentar";
            if(!Alumno.IsActive){
                Error.NonError = true;
                Error.Mensaje = "";
                _context.Alumnos.Remove(Alumno);
                _context.SaveChanges();
            }

        }
        return Json(Error);
    }

    public JsonResult ActivarDesactivarAlumno(int id){
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "No se encontro el alumno seleccionado";
        var Alumno = _context.Alumnos.Where(a => a.Id == id).FirstOrDefault();
        if (Alumno != null)
        {
            Error.NonError = true;
            Error.Mensaje = "";
            if (Alumno.IsActive)
            {
                Alumno.IsActive = false;
            }else{
                Alumno.IsActive = true;
            }
            _context.SaveChanges();
        }
        return Json(Error);
    }
}