using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class InventarioController : Controller
    {
        [HttpGet]
        public IActionResult MiBodega()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId == 2)
            {
                return RedirectToAction("MiInventario", "Tecnico");
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId == 2)
            {
                return RedirectToAction("MiInventario", "Tecnico");
            }
            return RedirectToAction("Dashboard", "Admin");
        }
    }
}

