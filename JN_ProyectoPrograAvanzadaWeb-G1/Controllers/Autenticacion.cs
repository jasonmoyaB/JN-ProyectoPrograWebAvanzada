using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Auth;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;
using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Helpers;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AutenticacionController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Login));

        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            try
            {
                var loginRequest = new LoginRequestDto
                {
                    CorreoElectronico = model.CorreoElectronico,
                    Contrasena = model.Contrasena
                };

                var loginResponse = await _authService.LoginAsync(loginRequest);

                if (loginResponse == null)
                {
                    ViewBag.Error = "Credenciales incorrectas o usuario inactivo.";
                    return View("Login", model);
                }

                HttpContext.Session.SetInt32("UsuarioID", loginResponse.UsuarioID);
                HttpContext.Session.SetString("Usuario", loginResponse.Nombre);
                HttpContext.Session.SetInt32("RolID", loginResponse.RolID);
                HttpContext.Session.SetString("RolNombre", loginResponse.RolNombre);
                
                if (loginResponse.BodegaID.HasValue)
                {
                    HttpContext.Session.SetInt32("BodegaID", loginResponse.BodegaID.Value);
                    HttpContext.Session.SetString("BodegaNombre", loginResponse.BodegaNombre ?? "");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginResponse.UsuarioID.ToString()),
                    new Claim(ClaimTypes.Name, loginResponse.Nombre),
                    new Claim(ClaimTypes.Email, loginResponse.CorreoElectronico),
                    new Claim(ClaimTypes.Role, loginResponse.RolNombre),
                    new Claim("RolID", loginResponse.RolID.ToString())
                };

                if (loginResponse.BodegaID.HasValue)
                {
                    claims.Add(new Claim("BodegaID", loginResponse.BodegaID.Value.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (loginResponse.RolID == 1)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (loginResponse.RolID == 2)
                {
                    return RedirectToAction("Dashboard", "Tecnico");
                }
                else
                {
                    return RedirectToAction("Main", "Home");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al iniciar sesión: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ViewBag.Error += $" Detalles: {ex.InnerException.Message}";
                }
                return View("Login", model);
            }
        }

        [HttpGet]
        public IActionResult Registro()
        {
           
            return View(); 
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

            var tecnico = _context.Roles.FirstOrDefault(r => r.NombreRol == "Técnico" || r.NombreRol == "Tecnico");
            if (tecnico == null)
            {
                ModelState.AddModelError("", "No se encontró el rol 'Técnico'. Configura los roles en el sistema.");
                return View("Registro", model);
            }

            var nuevo = new Usuario
            {
                Nombre = model.Nombre,
                CorreoElectronico = model.CorreoElectronico,
                ContrasenaHash = PasswordHelper.HashPassword(model.Contrasena),
                RolID = tecnico.RolID,
                Activo = true
            };

            _context.Usuarios.Add(nuevo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Usuario registrado correctamente.";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult RecuperarClave()
        {
            return View(); 
        }

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

            string nuevaClave = Guid.NewGuid().ToString("N")[..8]; 
            usuario.ContrasenaHash = PasswordHelper.HashPassword(nuevaClave);
            _context.SaveChanges();

            TempData["Mensaje"] = $"Tu nueva contraseña temporal es: {nuevaClave}. Por favor cámbiala después de iniciar sesión.";

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction(nameof(Login), "Autenticacion");
        }
    }
}
