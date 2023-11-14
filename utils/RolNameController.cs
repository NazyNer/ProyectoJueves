using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoJueves.Data;
using ProyectoJueves.Migrations;
using ProyectoJueves.Models;
namespace RolName.Utils;
public class RolNames : Controller
{
    private readonly ApplicationDbContext _context;


    public RolNames(ApplicationDbContext context)
    {
        _context = context;
    }
  public string RolNombre (string Id){
    var rolId = _context.UserRoles.Where(r => r.UserId == Id).Select(r => r.RoleId).FirstOrDefault();
    var RolName = _context.Roles.Where(r => r.Id == rolId).Select(r => r.Name).FirstOrDefault();
    return RolName;
  }
}