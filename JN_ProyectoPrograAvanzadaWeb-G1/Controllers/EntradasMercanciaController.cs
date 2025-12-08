using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class EntradasMercanciaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntradasMercanciaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // Cargar listas
        // ================================
        private void CargarListas()
        {
            ViewBag.Productos = new SelectList(_context.Productos.ToList(), "ProductoID", "Nombre");
            ViewBag.Bodegas = new SelectList(_context.Bodegas.ToList(), "BodegaID", "Nombre");
            ViewBag.Usuarios = new SelectList(_context.Usuarios.ToList(), "UsuarioID", "Nombre");
        }

        // ================================
        // LISTADO
        // ================================
        public async Task<IActionResult> Index()
        {
            var entradas = _context.EntradasMercancia
                .Include(e => e.Usuario)
                .Include(e => e.Producto)
                .Include(e => e.Bodega);

            return View(await entradas.ToListAsync());
        }

        // ================================
        // CREATE GET
        // ================================
        [HttpGet]
        public IActionResult Create()
        {
            CargarListas();
            return View(new EntradaMercancia());
        }

        // ================================
        // CREATE POST
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntradaMercancia entrada)
        {
            if (ModelState.IsValid)
            {
                entrada.FechaEntrada = DateTime.UtcNow;
                _context.Add(entrada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarListas();
            return View(entrada);
        }

        // ================================
        // EDIT GET
        // ================================
        public async Task<IActionResult> Edit(int id)
        {
            var entrada = await _context.EntradasMercancia.FindAsync(id);
            if (entrada == null) return NotFound();

            CargarListas();
            return View(entrada);
        }

        // ================================
        // EDIT POST
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EntradaMercancia entrada)
        {
            if (ModelState.IsValid)
            {
                _context.Update(entrada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarListas();
            return View(entrada);
        }

        // ================================
        // DELETE GET
        // ================================
        public async Task<IActionResult> Delete(int id)
        {
            var entrada = await _context.EntradasMercancia
                .Include(e => e.Usuario)
                .Include(e => e.Producto)
                .Include(e => e.Bodega)
                .FirstOrDefaultAsync(e => e.EntradaID == id);

            if (entrada == null) return NotFound();

            return View(entrada);
        }

        // ================================
        // DELETE POST
        // ================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrada = await _context.EntradasMercancia.FindAsync(id);

            if (entrada != null)
            {
                _context.Remove(entrada);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
