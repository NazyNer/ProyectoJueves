using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Models;
using Microsoft.AspNetCore.Identity;
using ProyectoJueves.Utils;

namespace ProyectoJueves.Controllers;

[Authorize(Roles = "Admin")]
public class ProfesorController : Controller
{

    private readonly ILogger<ProfesorController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ProfesorController(UserManager<IdentityUser> userManager,ApplicationDbContext context, ILogger<ProfesorController> logger)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }

    public JsonResult SearchProfesores(int Id)
    {
        var profesor = _context.Profesores.ToList();
        profesor = profesor.OrderBy(p => p.FullName).ToList();
        if (Id > 0)
        {
            profesor = profesor.Where(p => p.ProfesorId == Id).ToList();
        }
        return Json(profesor);
    }

    public async Task<JsonResult> SaveProfesor(int Id, string FullName, DateTime Birthdate, string Address, int Dni, string Email)
    {
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "Completar datos obligatorios";
        if (!string.IsNullOrEmpty(FullName) && Dni > 0 && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Email))
        {
            Error.Mensaje = "Por favor escribir un email real";
            if (IsValidEmail(Email))
            {
                if (Id == 0)
                {
                    Error.Mensaje = "Ya existe un profesor con ese DNI";
                    var ProfesorEmail = _context.Profesores.Where(p => p.Dni == Dni && p.EstadoProfesor != Estado.Eliminado).FirstOrDefault();
                    if (ProfesorEmail == null)
                    {
                        Error.Mensaje = "Ya existe un profesor con ese Email";
                        var usuarioExistente = await _userManager.FindByEmailAsync(Email);
                        if(usuarioExistente == null){
                            var profesor = new Profesor
                            {
                                Dni = Dni,
                                FullName = FullName,
                                Birthdate = Birthdate,
                                Address = Address,
                                Email = Email,
                                EstadoProfesor = Estado.Activo
                            };
                            _context.Profesores.Add(profesor);
                            _context.SaveChanges();
                            var user = new IdentityUser { UserName = FullName, Email = Email};
                            var contraseña = Dni.ToString();
                            var result = await _userManager.CreateAsync(user, contraseña);
                            if(result.Succeeded){
                                var Rol = _context.Roles.Where(r => r.Name == "Profesor").SingleOrDefault();
                                var usuarioAsignar = await _userManager.FindByEmailAsync(Email);
                                var Result = await _userManager.AddToRoleAsync(usuarioAsignar, Rol.Name);
                                if(Result.Succeeded){
                                    Error.NonError = true;
                                    Error.Mensaje = "";
                                }
                            }
                        }
                    }
                }
                else
                {
                    Error.Mensaje = "Ya existe un profesor con ese DNI";
                    var profesorDni = _context.Profesores.Where(p => p.ProfesorId != Id && p.Dni == Dni && p.EstadoProfesor != Estado.Eliminado).FirstOrDefault();
                    if (profesorDni == null)
                    {
                        Error.NonError = true;
                        Error.Mensaje = "";
                        var profesor = _context.Profesores.Where(p => p.ProfesorId == Id).FirstOrDefault();
                        profesor.Dni = Dni;
                        profesor.Address = Address;
                        profesor.Birthdate = Birthdate;
                        profesor.FullName = FullName;
                        profesor.EstadoProfesor = Estado.Activo;
                        _context.SaveChanges();
                    }
                }
            }
        }
        return Json(Error);
    }

    public JsonResult DeleteProfesor(int Id)
    {
        dynamic Error = new ExpandoObject();
        Error.NonError = false;
        Error.Mensaje = "No se selecciono ningun profesor";
        if (Id > 0)
        {
            var profesor = _context.Profesores.Where(p => p.ProfesorId == Id).FirstOrDefault();
            if (profesor != null)
            {
                var AsignaturasRelacionadas = _context.ProfesorAsignatura.Where(pa => pa.ProfesorID == Id).ToList();
                Error.Mensaje = "El profesor seleccionado tiene asignaturas relacionadas";
                if (AsignaturasRelacionadas.Count == 0)
                {
                    profesor.EstadoProfesor = Estado.Eliminado;
                    _context.SaveChanges();
                }
            }
        }
        return Json(Error);
    }

    public JsonResult Asignaturas(int id)
    {
        dynamic Resultado = new ExpandoObject();
        var Asignaturas = _context.Asignaturas.ToList();
        Resultado.asignaturas = Asignaturas;
        var AsignaturasRelacionadas = _context.ProfesorAsignatura.Where(pa => pa.ProfesorID == id).ToList();
        dynamic asignaturasEnRelacion = new ExpandoObject();
        foreach (var asignatura in AsignaturasRelacionadas)
        {
            var nombreAsignatura = Asignaturas.Where(a => a.AsignaturaId == asignatura.AsignaturaID).FirstOrDefault();
            ((IDictionary<string, object>)asignaturasEnRelacion)[asignatura.AsignaturaID.ToString()] = nombreAsignatura;
        }
        Resultado.asignaturasRelacionadas = asignaturasEnRelacion;
        return Json(Resultado);
    }

    public JsonResult GuardarAsignaturas(int[] AsignaturasJs, int ProfesorId)
    {
        dynamic Error = new ExpandoObject();
        dynamic AsiganturaSeleccionadas = new ExpandoObject();
        var asignaturas = _context.Asignaturas.ToList();
        // var AsiganturaSeleccionadas = _context.Asignaturas.Where(a => a.AsignaturaId == ).ToList();
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                // relaciones actuales del profesor
                var relacionesActuales = _context.ProfesorAsignatura.Where(pa => pa.ProfesorID == ProfesorId).ToList();

                // relaciones actuales
                foreach (var relacionActual in relacionesActuales)
                {
                    var asignaturaIdActual = relacionActual.AsignaturaID;

                    // asignatura actual no está en los parámetros
                    if (!AsignaturasJs.Contains(asignaturaIdActual))
                    {
                        //eliminar  relación
                        _context.ProfesorAsignatura.Remove(relacionActual);
                    }
                }
                // asignaturas pasadas como parámetros
                foreach (var asignaturaId in AsignaturasJs)
                {
                    // asignatura existe en la base de datos
                    var asignatura = _context.Asignaturas.FirstOrDefault(a => a.AsignaturaId == asignaturaId);
                    if (asignatura != null)
                    {
                        //  relación entre el profesor asignatura
                        var existente = _context.ProfesorAsignatura
                            .FirstOrDefault(pa => pa.ProfesorID == ProfesorId && pa.AsignaturaID == asignaturaId);

                        if (existente == null)
                        {
                            // crear relación
                            var profesorAsignatura = new ProfesorAsignatura
                            {
                                ProfesorID = ProfesorId,
                                AsignaturaID = asignaturaId,
                            };
                            _context.ProfesorAsignatura.Add(profesorAsignatura);
                        }
                    }
                    else
                    {
                        Error.mensaje = "No se encontró la asignatura.";
                        Error.NonError = false;
                        break;
                    }
                }
                Error.NonError = true;
                Error.mensaje = "";
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                Error[0] = e.Message;
                transaction.Rollback();
                return Json(Error);
            }
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
