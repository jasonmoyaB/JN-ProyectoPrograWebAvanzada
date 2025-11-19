using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
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
    }
}
