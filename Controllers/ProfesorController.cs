using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Models;


namespace ProyectoJueves.Controllers;
[Authorize]
public class ProfesorController : Controller {

    private readonly ILogger<ProfesorController> _logger;
    private readonly ApplicationDbContext _context;

    public ProfesorController(ApplicationDbContext context, ILogger<ProfesorController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    public JsonResult SearchProfesores(int Id){
        var profesor = _context.Profesores.ToList();
        profesor = profesor.OrderBy(p => p.FullName).ToList();
        if(Id > 0 ){
            profesor = profesor.Where(p => p.ProfesorId == Id).ToList();
        }
        return Json(profesor);
    }

    public JsonResult SaveProfesor(int Id, string FullName, DateTime Birthdate, string Address, int Dni, string Email){
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "Completar datos obligatorios";
        if(!string.IsNullOrEmpty(FullName) && Dni > 0 && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Email))
        {
            Error.Mensaje = "Por favor escribir un email real";
            if(IsValidEmail(Email))
            {
                if (Id == 0)
                {   
                    Error.Mensaje = "Ya existe un profesor con ese DNI";
                    var ProfesorEmail = _context.Profesores.Where(p => p.Dni == Dni).FirstOrDefault();
                    if (ProfesorEmail == null)
                    {
                        Error.NonError = true;
                        Error.Mensaje = "";
                        var profesor = new Profesor{
                            Dni = Dni,
                            FullName = FullName,
                            Birthdate = Birthdate,
                            Address = Address,
                            Email = Email
                        };
                        _context.Profesores.Add(profesor);
                        _context.SaveChanges();
                    }
                }else{
                    Error.Mensaje = "Ya existe un profesor con ese DNI";
                    var profesorDni = _context.Profesores.Where(p => p.ProfesorId != Id && p.Dni == Dni).FirstOrDefault();
                    if (profesorDni == null)
                    {
                        Error.NonError = true;
                        Error.Mensaje = "";
                        var profesor = _context.Profesores.Where(p => p.ProfesorId == Id).FirstOrDefault();
                        profesor.Email = Email;
                        profesor.Dni = Dni;
                        profesor.Address = Address;
                        profesor.Birthdate = Birthdate;
                        profesor.FullName = FullName;
                        _context.SaveChanges();
                    }
                }
            }
        }
        return Json(Error);
    }

    public JsonResult DeleteProfesor (int Id){
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "No se selecciono ningun profesor";
        if(Id > 0){
            var profesor = _context.Profesores.Where(p => p.ProfesorId == Id).FirstOrDefault();
            if(profesor != null){
                Error.NonError = true;
                Error.Mensaje = ""; 
                _context.Profesores.Remove(profesor);
                _context.SaveChanges();
            }
        }
        return Json(Error);
    }

    public JsonResult Asignaturas(int id){
        dynamic Resultado = new ExpandoObject();
        var Asignaturas = _context.Asignaturas.ToList();
        Resultado.asignaturas = Asignaturas;
        var AsignaturasRelacionadas = _context.ProfesorAsignatura.Where(pa => pa.ProfesorID == id).ToList();
        dynamic asignaturasEnRelacion = new ExpandoObject();
        foreach(var asignatura in AsignaturasRelacionadas){
            var nombreAsignatura = Asignaturas.Where(a => a.AsignaturaId == asignatura.AsignaturaID).FirstOrDefault();
            ((IDictionary<int, string>)asignaturasEnRelacion)[asignatura.AsignaturaID] = nombreAsignatura.Nombre;
        }
        Resultado.asignaturasRelacionadas = asignaturasEnRelacion;
        return Json(Resultado);
    }
    public JsonResult GuardarAsignaturas(int[] Asignaturas){

    }

    public static bool IsValidEmail(string email)
    {
        // Expresión regular para validar emails
        string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";

        // Verificar si el email coincide con el patrón
        return Regex.IsMatch(email, pattern);
    }
}
