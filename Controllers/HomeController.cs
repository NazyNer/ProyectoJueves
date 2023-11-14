using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoJueves.Data;
using ProyectoJueves.Models;

namespace ProyectoJueves.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _rolManager;
    private readonly UserManager<IdentityUser> _userManager;
    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, RoleManager<IdentityRole> rolManager, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _rolManager = rolManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        //CREAR ROLES SI NO EXISTEN
        var nombreRolCrearAdmin = _context.Roles.Where(r => r.Name == "Admin").SingleOrDefault();
        if (nombreRolCrearAdmin == null)
        {
            var roleResult = await _rolManager.CreateAsync(new IdentityRole("Admin"));
        }
        var nombreRolCrearProfesor = _context.Roles.Where(r => r.Name == "Profesor").SingleOrDefault();
        if (nombreRolCrearProfesor == null)
        {
            var roleResult = await _rolManager.CreateAsync(new IdentityRole("Profesor"));
        }
        var nombreRolCrearEstudiante = _context.Roles.Where(r => r.Name == "Estudiante").SingleOrDefault();
        if (nombreRolCrearEstudiante == null)
        {
            var roleResult = await _rolManager.CreateAsync(new IdentityRole("Estudiante"));
        }
        var Administrador = await _userManager.FindByEmailAsync("administrador@admin.com");
        if (Administrador == null)
        {
            var RolAdmin = _context.Roles.Where(r => r.Name == "Admin").SingleOrDefault();
            var user = new IdentityUser { UserName = "administrador@admin.com", Email = "administrador@admin.com"};
            var result = await _userManager.CreateAsync(user, "administrador");
            if(result.Succeeded){
                var usuarioAdmin = await _userManager.FindByEmailAsync("administrador@admin.com");
                var asignarRolResult = await _userManager.AddToRoleAsync(usuarioAdmin, RolAdmin.Name);
            }
        }

        var profesores = _context.Profesores.ToList();
        var RolProfe = _context.Roles.Where(r => r.Name == "Profesor").SingleOrDefault();
        foreach (var profesor in profesores)
        {
            var ProfesorCreado = await _userManager.FindByEmailAsync(profesor.Email);
            if(ProfesorCreado == null){
                var user = new IdentityUser { UserName = profesor.Email, Email = profesor.Email};
                var contraseña = profesor.Dni.ToString();
                var ProfesorCrear = await _userManager.CreateAsync(user, contraseña);
                if(ProfesorCrear.Succeeded){
                    var usuarioCreado = await _userManager.FindByEmailAsync(profesor.Email);
                    var RolResult = await _userManager.AddToRoleAsync(usuarioCreado, RolProfe.Name);
                }
                profesor.UsuarioID = user.Id;
                _context.SaveChanges();
            }
        }
        var estudiantes = _context.Alumnos.ToList();
        var RolEstudiante  = _context.Roles.Where(r => r.Name == "Estudiante").SingleOrDefault();
        foreach (var estudiante in estudiantes)
        {
            var EstudianteCreado = await _userManager.FindByEmailAsync(estudiante.Email);
            if(EstudianteCreado == null){
                var user = new IdentityUser { UserName = estudiante.Email, Email = estudiante.Email};
                var contraseña = estudiante.DNI.ToString();
                var EstudianteCrear = await _userManager.CreateAsync(user, contraseña);
                if(EstudianteCrear.Succeeded){
                    var usuarioCreado = await _userManager.FindByEmailAsync(estudiante.Email);
                    var RolResult = await _userManager.AddToRoleAsync(usuarioCreado, RolEstudiante.Name);
                }
                estudiante.UsuarioID = user.Id;
                _context.SaveChanges();
            }
        }
        
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
