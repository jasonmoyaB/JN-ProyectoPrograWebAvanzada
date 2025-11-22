using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Usuarios;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolRepository _rolRepository;
        private readonly IBodegaRepository _bodegaRepository;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IUsuarioService usuarioService,
            IRolRepository rolRepository,
            IBodegaRepository bodegaRepository,
            ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _rolRepository = rolRepository;
            _bodegaRepository = bodegaRepository;
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

            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                return View(dto);
            }

            try
            {
                var actualizado = await _usuarioService.UpdateAsync(id, dto);
                if (!actualizado)
                {
                    TempData["Error"] = "Error al actualizar el usuario";
                    await CargarListasDesplegables();
                    return View(dto);
                }

                TempData["Success"] = "Usuario actualizado exitosamente";
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
                _logger.LogError(ex, "Error al actualizar usuario: {Id}", id);
                TempData["Error"] = "Error al actualizar el usuario";
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
                var actualizado = await _usuarioService.ToggleActivoAsync(id);
                if (!actualizado)
                {
                    TempData["Error"] = "Error al cambiar el estado del usuario";
                }
                else
                {
                    TempData["Success"] = "Estado del usuario actualizado";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de usuario: {Id}", id);
                TempData["Error"] = "Error al cambiar el estado del usuario";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasDesplegables()
        {
            var roles = await _rolRepository.GetAllAsync();
            var bodegas = await _bodegaRepository.GetAllAsync(true);

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




