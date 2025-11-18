using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class TecnicoController : Controller
    {
        private readonly ILogger<TecnicoController> _logger;

        public TecnicoController(ILogger<TecnicoController> logger)
        {
            _logger = logger;
        }

    
        /// Dashboard principal para Técnicos
        /// Solo accesible para usuarios con RolID = 2 (Técnico)
      
        [HttpGet]
        public IActionResult Dashboard()
        {
          
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 2)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.UsuarioNombre = HttpContext.Session.GetString("Usuario") ?? "Técnico";
            ViewBag.RolNombre = HttpContext.Session.GetString("RolNombre") ?? "Técnico";
            ViewBag.BodegaID = HttpContext.Session.GetInt32("BodegaID");
            ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Sin bodega asignada";
            
            return View();
        }
    }
}

