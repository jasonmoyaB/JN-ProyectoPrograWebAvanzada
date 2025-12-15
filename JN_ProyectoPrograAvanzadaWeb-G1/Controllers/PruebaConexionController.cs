

using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaWeb_G1.Data;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class PruebaConexionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PruebaConexionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var ok = await _context.Database.CanConnectAsync();

                if (ok)
                    return Content(" Conexión OK a SQL Server Local (DBInventario).");
                else
                    return Content(" No se pudo conectar a la base de datos.");
            }
            catch (Exception ex)
            {
                return Content("❌ Error al conectar: " + ex.Message);
            }
        }
    }
}

