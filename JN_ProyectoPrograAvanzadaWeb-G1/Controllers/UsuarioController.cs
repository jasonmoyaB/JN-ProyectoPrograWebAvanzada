using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Helpers;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
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

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult CambiarContrasena(CambiarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == model.UsuarioID);
            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            string hashActual = PasswordHelper.HashPassword(model.ContrasenaActual);
            if (usuario.ContrasenaHash != hashActual)
            {
                ModelState.AddModelError("", "La contraseña actual es incorrecta.");
                return View(model);
            }

            usuario.ContrasenaHash = PasswordHelper.HashPassword(model.NuevaContrasena);
            _context.SaveChanges();

            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Perfil");
        }

        // PANEL DE ADMINISTRACIÓN
        [HttpGet]
        public IActionResult Gestion()
        {
            int? rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
                return RedirectToAction("Main", "Home");

            var usuarios = _context.Usuarios.Include(u => u.Rol).ToList();
            return View("Gestion", usuarios); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleEstado(int id)
        {
            int? rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
                return RedirectToAction("Main", "Home");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == id);
            if (usuario == null)
                return NotFound();

            usuario.Activo = !usuario.Activo;
            _context.SaveChanges();

            TempData["Mensaje"] = $"El usuario ahora está {(usuario.Activo ? "activo" : "inactivo")}.";
            return RedirectToAction("Gestion");
        }

        // EDICIÓN ADMINISTRATIVA
        [HttpGet]
        public IActionResult Editar(int id)
        {
            int? rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
                return RedirectToAction("Main", "Home");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == id);
            if (usuario == null)
                return NotFound();

            var model = new UsuarioEdicionViewModel
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                RolID = usuario.RolID,
                Activo = usuario.Activo,
                RolesDisponibles = _context.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.RolID.ToString(),
                        Text = r.NombreRol
                    }).ToList()
            };

            model.RolesDisponibles.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Seleccione un rol --",
                Disabled = true,
                Selected = model.RolID == 0
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(UsuarioEdicionViewModel model)
        {
            int? rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
                return RedirectToAction("Main", "Home");

            ModelState.Remove(nameof(model.RolesDisponibles)); 

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == model.UsuarioID);
            if (usuario == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Mensaje"] = "Errores: " + string.Join(" | ", errores);

                model.RolesDisponibles = _context.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.RolID.ToString(),
                        Text = r.NombreRol
                    }).ToList();

                model.RolesDisponibles.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "-- Seleccione un rol --",
                    Disabled = true,
                    Selected = model.RolID == 0
                });

                return View(model);
            }

            usuario.Nombre = model.Nombre;
            usuario.CorreoElectronico = model.CorreoElectronico;
            usuario.RolID = model.RolID;
            usuario.Activo = model.Activo;

            _context.SaveChanges();

            // Actualizar nombre en sesión si el usuario editado es el mismo que está logueado
            if (HttpContext.Session.GetInt32("UsuarioID") == usuario.UsuarioID)
            {
                HttpContext.Session.SetString("Usuario", usuario.Nombre);
            }

            TempData["Mensaje"] = "Usuario actualizado correctamente.";
            return RedirectToAction("Gestion");
        }

    }
}
