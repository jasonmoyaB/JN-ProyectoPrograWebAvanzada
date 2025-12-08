using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApiProductoService _productoService;
        private readonly IApiBodegaService _bodegaService;

        public AuditoriaController(
            ApplicationDbContext context,
            IApiProductoService productoService,
            IApiBodegaService bodegaService)
        {
            _context = context;
            _productoService = productoService;
            _bodegaService = bodegaService;
        }

     
        public async Task<IActionResult> ReportesKPI()
        {
            var usuarios = await _context.Usuarios
                                         .Include(u => u.Rol)
                                         .OrderBy(u => u.Nombre)
                                         .ToListAsync();

            return View(usuarios);
        }
        
        public async Task<IActionResult> Bitacora()
        {
            var productos = await _productoService.GetAllAsync();
            var bodegas = await _bodegaService.GetAllAsync();

            var vm = new BitacoraViewModel
            {
                Productos = productos.OrderBy(p => p.Nombre).ToList(),
                Bodegas = bodegas.OrderBy(b => b.Nombre).ToList()
            };

            return View(vm);
        }
    }
}
