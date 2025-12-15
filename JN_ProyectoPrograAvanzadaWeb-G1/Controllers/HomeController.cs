using System.Diagnostics;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId.HasValue)
            {
                if (rolId == 1) // Administrador
                    return RedirectToAction("Dashboard", "Admin");
                else if (rolId == 2) // Técnico
                    return RedirectToAction("Dashboard", "Tecnico");
            }
            
            return View(new JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels.LoginViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Main()
        {
            // Redirigir  el rol del usuario
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId == 1) // Administrador
                return RedirectToAction("Dashboard", "Admin");
            else if (rolId == 2) // Técnico
                return RedirectToAction("Dashboard", "Tecnico");
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
