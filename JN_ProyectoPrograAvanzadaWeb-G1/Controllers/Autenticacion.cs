using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Helpers;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AutenticacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Login));

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // -> Views/Autenticacion/Login.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            string hash = PasswordHelper.HashPassword(model.Contrasena);
            var usuario = _context.Usuarios.FirstOrDefault(u =>
                u.CorreoElectronico == model.CorreoElectronico &&
                u.ContrasenaHash == hash &&
                u.Activo == true);

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View("Login", model);
            }

            HttpContext.Session.SetString("Usuario", usuario.Nombre);
            HttpContext.Session.SetInt32("RolID", usuario.RolID);

            // ✅ Redirección correcta al Home/Main
            return RedirectToAction("Main", "Home");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            // Ya no necesitas cargar roles
            return View(); // -> Views/Autenticacion/Registro.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Registro", model);
            }

            if (_context.Usuarios.Any(u => u.CorreoElectronico == model.CorreoElectronico))
            {
                ModelState.AddModelError("", "El correo ya está registrado.");
                return View("Registro", model);
            }

            // 🔒 Forzar que todo nuevo usuario sea "Vendedor"
            var vendedor = _context.Roles.FirstOrDefault(r => r.NombreRol == "Vendedor");
            if (vendedor == null)
            {
                ModelState.AddModelError("", "No se encontró el rol 'Vendedor'. Configura los roles en el sistema.");
                return View("Registro", model);
            }

            var nuevo = new Usuario
            {
                Nombre = model.Nombre,
                CorreoElectronico = model.CorreoElectronico,
                ContrasenaHash = PasswordHelper.HashPassword(model.Contrasena),
                RolID = vendedor.RolID, // <- se ignora model.RolID y se fuerza Vendedor
                Activo = true
            };

            _context.Usuarios.Add(nuevo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Usuario registrado correctamente.";
            return RedirectToAction(nameof(Login));
        }
        // GET: /Autenticacion/RecuperarClave
        [HttpGet]
        public IActionResult RecuperarClave()
        {
            return View(); // -> Views/Autenticacion/RecuperarClave.cshtml
        }

        // POST: /Autenticacion/RecuperarClave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecuperarClave(RecuperarClaveViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CorreoElectronico))
            {
                ViewBag.Error = "Por favor ingresa tu correo electrónico.";
                return View(model);
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.CorreoElectronico == model.CorreoElectronico);

            if (usuario == null)
            {
                ViewBag.Error = "No existe una cuenta con ese correo.";
                return View(model);
            }

            // 🔑 Generar nueva contraseña temporal
            string nuevaClave = Guid.NewGuid().ToString("N")[..8]; // 8 caracteres aleatorios
            usuario.ContrasenaHash = PasswordHelper.HashPassword(nuevaClave);
            _context.SaveChanges();

            // 📩 Simular envío de correo (aquí podrías integrar SMTP si quieres)
            TempData["Mensaje"] = $"Tu nueva contraseña temporal es: {nuevaClave}. Por favor cámbiala después de iniciar sesión.";

            return RedirectToAction(nameof(Login));
        }


        public IActionResult Logout()
        {
            // 1️⃣ Limpiar toda la sesión activa del usuario
            HttpContext.Session.Clear();

            // 2️⃣ Cerrar cualquier cookie de autenticación activa
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 3️⃣ Invalidar la caché del navegador para que no se pueda retroceder con “Atrás”
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // 4️⃣ Redirigir de forma segura al login
            return RedirectToAction(nameof(Login), "Autenticacion");
        }
    }
}
