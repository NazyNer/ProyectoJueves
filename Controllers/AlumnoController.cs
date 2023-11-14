using System.Diagnostics;
using System.Dynamic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProyectoJueves.Data;
using ProyectoJueves.Models;
using ProyectoJueves.Utils;


namespace ProyectoJueves.Controllers;


[Authorize(Roles = "Admin")]
public class AlumnoController : Controller {

    private readonly ILogger<AlumnoController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public AlumnoController(UserManager<IdentityUser> userManager,ApplicationDbContext context, ILogger<AlumnoController> logger)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    
    public IActionResult Index()
    {
        var Carrera = _context.Carreras?.Where(c => c.EstadoCarrera == Estado.Activo).ToList();
        ViewBag.CarreraID = new SelectList(Carrera?.OrderBy(p => p.Name), "Id", "Name");
        return View();
    }

    public JsonResult SearchStudents(int Id)
    {
        var alumnos = _context.Alumnos.Where(a => a.EstadoAlumno != Estado.Eliminado).ToList();
        alumnos = alumnos.OrderBy(a =>a.EstadoAlumno).ThenBy(a =>a.CarreraName).ThenBy(a => a.FullName).ToList();
        if(Id > 0){
            alumnos = alumnos.Where(a => a.Id == Id).ToList();
        }
        return Json(alumnos);
    }

    public async Task<JsonResult> GuardarAlumno(int Id, string FullName, int Dni, string Email, string Address, DateTime Birthdate, int CarreraId)
    {
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "Completar datos obligatorios";
        // validacion de que todos los datos obligatorios sean ingresados
        if (!string.IsNullOrEmpty(FullName) && CarreraId > 0)
        {
            Error.Mensaje = "Por favor escribir un email real";
            if (IsValidEmail(Email))
            {    
                Error.NonError = false;
                Error.Mensaje = "Carrera no encontrada";
                var carreraOriginal = _context.Carreras?.Where(c => c.Id == CarreraId && c.EstadoCarrera == Estado.Activo).FirstOrDefault();
                if (carreraOriginal != null)
                {
                    var Alumnos = ((JsonResult)SearchStudents(0)).Value as List<Alumno>;
                    if (Id == 0)
                    {
                        Error.Mensaje = "Ya existe un alumno con ese DNI";
                        var AlumnoDNI = Alumnos.Where(p => p.DNI == Dni).FirstOrDefault();
                        if (AlumnoDNI == null)
                        {
                            Error.Mensaje = "Ya existe un alumno con ese EMAIL";
                            var AlumnoEmail = Alumnos.Where(p => p.Email == Email).FirstOrDefault();
                            if (AlumnoEmail == null){
                                //Crear Alumno
                                var Alumno = new Alumno{
                                FullName = FullName,
                                Birthdate = Birthdate,
                                CarreraId = carreraOriginal.Id,
                                CarreraName = carreraOriginal.Name,
                                DNI = Dni,
                                Email = Email,
                                Address = Address,
                                EstadoAlumno = Estado.Activo
                                };
                                _context.Alumnos.Add(Alumno);
                                _context.SaveChanges();

                                var user = new IdentityUser { UserName = Email, Email = Email};
                                var contraseña = Dni.ToString();
                                var result = await _userManager.CreateAsync(user, contraseña);
                                if(result.Succeeded){
                                    var Rol = _context.Roles.Where(r => r.Name == "Estudiante").SingleOrDefault();
                                    var usuarioAsignar = await _userManager.FindByEmailAsync(Email);
                                    var Result = await _userManager.AddToRoleAsync(usuarioAsignar, Rol.Name);
                                    if(Result.Succeeded){
                                        Error.NonError = true;
                                        Error.Mensaje = "Datos guardados correctamente";
                                    }
                                }
                            }
                        }
                    }else{
                        Error.Mensaje = "Ya existe un alumno con ese DNI";
                        var AlumnoEmail = Alumnos.Where(p => p.DNI == Dni && p.Id != Id).FirstOrDefault();
                        if (AlumnoEmail == null)
                        {
                            //Editar Alumno
                            Error.Mensaje = "No se encontro el alumno seleccionado";
                            var Alumno = Alumnos.Where(a => a.Id == Id).FirstOrDefault();
                            if (Alumno != null)
                            {
                                Alumno.FullName = FullName;
                                Alumno.Birthdate = Birthdate;
                                Alumno.CarreraId = carreraOriginal.Id;
                                Alumno.CarreraName = carreraOriginal.Name;
                                Alumno.DNI = Dni;
                                Alumno.Address = Address;
                                Alumno.EstadoAlumno = Estado.Activo;
                                _context.SaveChanges();
                                Error.NonError = true;
                                Error.Mensaje = "Datos guardados correctamente";
                            }
                            
                        }
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
            if(Alumno.EstadoAlumno == Estado.Desactivado){
                Error.NonError = true;
                Error.Mensaje = "";
                Alumno.EstadoAlumno = Estado.Eliminado;
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
            if (Alumno.EstadoAlumno == Estado.Activo)
            {
                Alumno.EstadoAlumno = Estado.Desactivado;
            }else if (Alumno.EstadoAlumno == Estado.Desactivado)
            {
                Alumno.EstadoAlumno = Estado.Activo;
            }
            _context.SaveChanges();
        }
        return Json(Error);
    }
    public static bool IsValidEmail(string email)
    {
        // Expresión regular para validar emails
        string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";

        // Verificar si el email coincide con el patrón
        return Regex.IsMatch(email, pattern);
    }
}