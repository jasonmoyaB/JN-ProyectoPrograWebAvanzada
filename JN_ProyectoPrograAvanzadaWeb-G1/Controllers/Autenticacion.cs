using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly IApiAuthService _authService;
        private readonly IApiUsuarioService _usuarioService;
        private readonly IApiRolService _rolService;
        private readonly ILogger<AutenticacionController> _logger;

        public AutenticacionController(
            IApiAuthService authService,
            IApiUsuarioService usuarioService,
            IApiRolService rolService,
            ILogger<AutenticacionController> logger)
        {
            _authService = authService;
            _usuarioService = usuarioService;
            _rolService = rolService;
            _logger = logger;
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
        public async Task<IActionResult> Registrar(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Registro", model);
            }

            try
            {
                // Verificar si el correo ya existe
                var usuarios = await _usuarioService.GetAllAsync();
                if (usuarios.Any(u => u.CorreoElectronico == model.CorreoElectronico))
                {
                    ModelState.AddModelError("", "El correo ya está registrado.");
                    return View("Registro", model);
                }

                // Obtener rol Tcnico
                var roles = await _rolService.GetAllAsync();
                var tecnico = roles.FirstOrDefault(r => r.NombreRol == "Técnico" || r.NombreRol == "Tecnico");
                if (tecnico == null)
                {
                    ModelState.AddModelError("", "No se encontró el rol 'Técnico'. Configura los roles en el sistema.");
                    return View("Registro", model);
                }

                // Crear usuario a través del API
                var crearUsuarioDto = new CrearUsuarioDto
                {
                    Nombre = model.Nombre,
                    CorreoElectronico = model.CorreoElectronico,
                    Contrasena = model.Contrasena,
                    RolID = tecnico.RolID,
                    BodegaID = null
                };

                var usuarioId = await _usuarioService.CreateAsync(crearUsuarioDto);
                if (usuarioId > 0)
                {
                    TempData["Mensaje"] = "Usuario registrado correctamente.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    _logger.LogWarning("CreateAsync retornó 0 para usuario: {Email}", model.CorreoElectronico);
                    ModelState.AddModelError("", "Error al registrar el usuario. Verifique que el API esté ejecutándose y que los datos sean correctos.");
                    return View("Registro", model);
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de operación al registrar usuario");
                ModelState.AddModelError("", ex.Message);
                return View("Registro", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar usuario: {Message}", ex.Message);
                ModelState.AddModelError("", $"Error al registrar el usuario: {ex.Message}");
                return View("Registro", model);
            }
        }
        [HttpGet]
        public IActionResult RecuperarClave()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarClave(RecuperarClaveViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CorreoElectronico))
            {
                ViewBag.Error = "Por favor ingresa tu correo electrónico.";
                return View(model);
            }

            try
            {
                var nuevaClave = await _authService.RecuperarClaveAsync(model.CorreoElectronico);
                
                if (string.IsNullOrEmpty(nuevaClave))
                {
                    ViewBag.Error = "No existe una cuenta con ese correo o hubo un error al generar la contraseña temporal.";
                    return View(model);
                }

                TempData["Mensaje"] = $"Tu nueva contraseña temporal es: {nuevaClave}. Por favor cámbiala después de iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar clave");
                ViewBag.Error = "Error al recuperar la contraseña. Intente nuevamente.";
                return View(model);
            }
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
