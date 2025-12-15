using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApiAuthService _authService;

        public UsuarioController(ApplicationDbContext context, IApiAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // PERFIL PERSONAL
        [HttpGet]
        public IActionResult Perfil()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var model = new UsuarioPerfilViewModel
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                Rol = usuario.Rol?.NombreRol ?? "Sin rol",
                Activo = usuario.Activo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Perfil(UsuarioPerfilViewModel model)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);
            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Error de validación.";
                model.Rol = _context.Roles.FirstOrDefault(r => r.RolID == usuario.RolID)?.NombreRol ?? "Sin rol";
                return View(model);
            }

            usuario.Nombre = model.Nombre;
            usuario.CorreoElectronico = model.CorreoElectronico;
            usuario.Activo = model.Activo;
            _context.SaveChanges();

            HttpContext.Session.SetString("Usuario", usuario.Nombre);

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Perfil");
        }

        // CAMBIO DE CONTRASEÑA
        [HttpGet]
        public IActionResult CambiarContrasena()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);
            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            return View(new CambiarContrasenaViewModel { UsuarioID = usuario.UsuarioID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == model.UsuarioID);
            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var resultado = await _authService.CambiarContrasenaAsync(
                model.UsuarioID, 
                model.ContrasenaActual, 
                model.NuevaContrasena);

            if (!resultado)
            {
                ModelState.AddModelError("", "La contraseña actual es incorrecta o no se pudo cambiar la contraseña.");
                return View(model);
            }

            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Perfil");
        }


    }
}
