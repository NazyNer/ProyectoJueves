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
    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, RoleManager<IdentityRole> rolManager)
    {
        _logger = logger;
        _context = context;
        _rolManager = rolManager;
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
