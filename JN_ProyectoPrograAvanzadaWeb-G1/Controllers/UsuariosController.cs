using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IApiUsuarioService _usuarioService;
        private readonly IApiRolService _rolService;
        private readonly IApiBodegaService _bodegaService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IApiUsuarioService usuarioService,
            IApiRolService rolService,
            IApiBodegaService bodegaService,
            ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _bodegaService = bodegaService;
            _logger = logger;
        }

        // GET: Usuarios
        [HttpGet]
        public async Task<IActionResult> Index(bool? activo = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                var usuarios = await _usuarioService.GetAllAsync(activo);
                ViewBag.FiltroActivo = activo;
                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                TempData["Error"] = "Error al cargar los usuarios";
                return View(new List<UsuarioDto>());
            }
        }

        // GET: Usuarios/Crear
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            await CargarListasDesplegables();
            return View(new CrearUsuarioDto());
        }

        // POST: Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearUsuarioDto dto)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                return View(dto);
            }

            try
            {
                await _usuarioService.CreateAsync(dto);
                TempData["Success"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarListasDesplegables();
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                TempData["Error"] = "Error al crear el usuario";
                await CargarListasDesplegables();
                return View(dto);
            }
        }

        // GET: Usuarios/Editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var dto = new EditarUsuarioDto
                {
                    UsuarioID = usuario.UsuarioID,
                    Nombre = usuario.Nombre,
                    CorreoElectronico = usuario.CorreoElectronico,
                    RolID = usuario.RolID,
                    BodegaID = usuario.BodegaID,
                    Activo = usuario.Activo
                };

                await CargarListasDesplegables();
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario: {Id}", id);
                TempData["Error"] = "Error al cargar el usuario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Usuarios/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, EditarUsuarioDto dto)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }


            if (dto.BodegaID.HasValue && dto.BodegaID.Value <= 0)
            {
                dto.BodegaID = null;
            }
            
            
            if (Request.Form.TryGetValue("BodegaID", out var bodegaIdFormValue))
            {
                if (string.IsNullOrWhiteSpace(bodegaIdFormValue) || bodegaIdFormValue == "0")
                {
                    dto.BodegaID = null;
                }
            }

           
            dto.UsuarioID = id;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido al editar usuario {Id}. Errores: {Errors}", 
                    id, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                await CargarListasDesplegables();
                return View(dto);
            }

            try
            {
                _logger.LogInformation("Actualizando usuario {Id} con datos: Nombre={Nombre}, RolID={RolID}, BodegaID={BodegaID}, Activo={Activo}", 
                    id, dto.Nombre, dto.RolID, dto.BodegaID, dto.Activo);
                
                var actualizado = await _usuarioService.UpdateAsync(id, dto);
                

                if (actualizado)
                {
                    _logger.LogInformation("Usuario {Id} actualizado exitosamente, redirigiendo al catálogo", id);
                    TempData["Success"] = "Usuario actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                

                _logger.LogWarning("UpdateAsync retornó false para usuario {Id} - Usuario no encontrado", id);
                ModelState.AddModelError("", "El usuario no fue encontrado. Verifique que el usuario exista.");
                TempData["Error"] = "Error al actualizar el usuario. El usuario no fue encontrado.";
                await CargarListasDesplegables();
                return View(dto);
            }
            catch (InvalidOperationException ex)
            {
              
                _logger.LogError(ex, "Error de validación al actualizar usuario {Id}: {Message}", id, ex.Message);
                ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message;
                await CargarListasDesplegables();
                return View(dto);
            }
            catch (HttpRequestException ex)
            {
                
                _logger.LogError(ex, "Error de conexión al actualizar usuario {Id}: {Message}", id, ex.Message);
                var errorMessage = "No se pudo conectar con el servidor. Verifique su conexión e intente nuevamente.";
                ModelState.AddModelError("", errorMessage);
                TempData["Error"] = errorMessage;
                await CargarListasDesplegables();
                return View(dto);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Error inesperado al actualizar usuario {Id}: {Message}", id, ex.Message);
                var errorMessage = "Error al actualizar el usuario. Por favor, intente nuevamente.";
                ModelState.AddModelError("", errorMessage);
                TempData["Error"] = errorMessage;
                await CargarListasDesplegables();
                return View(dto);
            }
        }

        // POST: Usuarios/ToggleActivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                _logger.LogInformation("Cambiando estado de usuario {Id}", id);
                
                
                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Usuario {Id} no encontrado al intentar cambiar estado", id);
                    TempData["Error"] = "El usuario no fue encontrado";
                    return RedirectToAction(nameof(Index));
                }
                
                var estadoAnterior = usuario.Activo;
                var actualizado = await _usuarioService.ToggleActivoAsync(id);
                
                if (!actualizado)
                {
                    _logger.LogWarning("ToggleActivoAsync retornó false para usuario {Id}", id);
                    TempData["Error"] = "Error al cambiar el estado del usuario. Verifique que el usuario exista.";
                }
                else
                {
                    var nuevoEstado = !estadoAnterior ? "activado" : "desactivado";
                    _logger.LogInformation("Usuario {Id} {NuevoEstado} exitosamente", id, nuevoEstado);
                    TempData["Success"] = $"Usuario {nuevoEstado} exitosamente";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al cambiar estado de usuario {Id}: {Message}", id, ex.Message);
                TempData["Error"] = "No se pudo conectar con el servidor. Verifique su conexión e intente nuevamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al cambiar estado de usuario {Id}: {Message}", id, ex.Message);
                TempData["Error"] = "Error al cambiar el estado del usuario. Por favor, intente nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasDesplegables()
        {
            var roles = await _rolService.GetAllAsync();
            var bodegas = await _bodegaService.GetAllAsync(true);

            ViewBag.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.RolID.ToString(),
                Text = r.NombreRol
            }).ToList();

            ViewBag.Bodegas = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Sin bodega --", Selected = true }
            }.Concat(bodegas.Select(b => new SelectListItem
            {
                Value = b.BodegaID.ToString(),
                Text = b.Nombre
            })).ToList();
        }
    }
}




