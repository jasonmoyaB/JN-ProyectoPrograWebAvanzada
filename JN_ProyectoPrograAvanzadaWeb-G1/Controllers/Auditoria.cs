using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Bodegas;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;
using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaController(ApplicationDbContext context)
        {
            _context = context;
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
            var productos = await _context.Productos
                .OrderBy(p => p.Nombre)
                .Select(p => new ProductoDto
                {
                    ProductoID = p.ProductoID,
                    SKU = p.SKU,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    EsSerializado = p.EsSerializado,
                    Activo = p.Activo,
                    FechaCreacion = p.FechaCreacion
                })
                .ToListAsync();

            var bodegas = await _context.Bodegas
                .OrderBy(b => b.Nombre)
                .Select(b => new BodegaDto
                {
                    BodegaID = b.BodegaID,
                    Nombre = b.Nombre,
                    Ubicacion = b.Ubicacion,
                    Activo = b.Activo,
                    FechaCreacion = b.FechaCreacion,
                    
                })
                .ToListAsync();

            var vm = new BitacoraViewModel
            {
                Productos = productos,
                Bodegas = bodegas
            };

            return View(vm);
        }
    }
}
