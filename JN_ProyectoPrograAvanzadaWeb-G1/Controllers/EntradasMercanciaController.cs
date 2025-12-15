using JN_ProyectoPrograAvanzadaWeb_G1.Data;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    public class EntradasMercanciaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntradasMercanciaController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // Cargar 
        
        private void CargarListas()
        {
            ViewBag.Productos = new SelectList(_context.Productos.ToList(), "ProductoID", "Nombre");
            ViewBag.Bodegas = new SelectList(_context.Bodegas.ToList(), "BodegaID", "Nombre");
            ViewBag.Usuarios = new SelectList(_context.Usuarios.ToList(), "UsuarioID", "Nombre");
        }

        
        // LISTADO
       
        public async Task<IActionResult> Index()
        {
            try
            {
                var entradas = _context.EntradasMercancia
                    .Include(e => e.Usuario)
                    .Include(e => e.Producto)
                    .Include(e => e.Bodega)
                    .Include(e => e.TipoMovimientoNav)
                    .OrderByDescending(e => e.FechaEntrada);

                return View(await entradas.ToListAsync());
            }
            catch (Exception ex)
            {
                // Log del error
                TempData["Error"] = "Error al cargar las entradas de mercancía: " + ex.Message;
                return View(new List<EntradaMercancia>());
            }
        }

        
        //  GET
        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Obtener usuario logueado 
                var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
                if (!usuarioId.HasValue)
                {
                    TempData["Error"] = "Debe iniciar sesión para crear una entrada de mercancía";
                    return RedirectToAction("Login", "Autenticacion");
                }

                // Obtener información 
                var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
                var bodegaGeneral = await _context.Bodegas.FindAsync(1);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Index");
                }

                if (bodegaGeneral == null)
                {
                    TempData["Error"] = "Bodega General no encontrada. Por favor, contacte al administrador.";
                    return RedirectToAction("Index");
                }

                ViewBag.UsuarioID = usuarioId.Value;
                ViewBag.UsuarioNombre = usuario.Nombre;
                ViewBag.BodegaID = 1;
                ViewBag.BodegaNombre = bodegaGeneral.Nombre;

                // Cargar solo productos activos
                var productos = await _context.Productos.Where(p => p.Activo).ToListAsync();
                ViewBag.Productos = new SelectList(productos, "ProductoID", "Nombre");
                
                // Cargar o crear tipos de movimiento: Compra y Devolución
                var tiposMovimiento = new List<TipoMovimiento>();
                
                // Buscar o crear tipo Compra
                var tipoCompra = await _context.Set<TipoMovimiento>()
                    .FirstOrDefaultAsync(tm => tm.Codigo == "Compra");
                if (tipoCompra == null)
                {
                    tipoCompra = new TipoMovimiento { Codigo = "Compra", Naturaleza = 1 };
                    _context.Set<TipoMovimiento>().Add(tipoCompra);
                    await _context.SaveChangesAsync();
                }
                tiposMovimiento.Add(tipoCompra);
                
                // Buscar o crear tipo Devolucion
                var tipoDevolucion = await _context.Set<TipoMovimiento>()
                    .FirstOrDefaultAsync(tm => tm.Codigo == "Devolucion");
                if (tipoDevolucion == null)
                {
                    tipoDevolucion = new TipoMovimiento { Codigo = "Devolucion", Naturaleza = 1 };
                    _context.Set<TipoMovimiento>().Add(tipoDevolucion);
                    await _context.SaveChangesAsync();
                }
                tiposMovimiento.Add(tipoDevolucion);
                
                ViewBag.TiposMovimiento = new SelectList(tiposMovimiento, "TipoMovimientoID", "Codigo");

                var entrada = new EntradaMercancia
                {
                    UsuarioID = usuarioId.Value,
                    BodegaID = 1
                };

                return View(entrada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el formulario: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        
        //  POST
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntradaMercancia entrada)
        {
            try
            {
                
                var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
                if (!usuarioId.HasValue)
                {
                    TempData["Error"] = "Debe iniciar sesión para crear una entrada de mercancía";
                    return RedirectToAction("Login", "Autenticacion");
                }

                entrada.UsuarioID = usuarioId.Value;
                entrada.BodegaID = 1; 

                
                var producto = await _context.Productos.FindAsync(entrada.ProductoID);
                if (producto == null)
                {
                    ModelState.AddModelError("ProductoID", "El producto seleccionado no existe");
                }
                else if (!producto.Activo)
                {
                    ModelState.AddModelError("ProductoID", "El producto seleccionado no está activo");
                }

                
                var usuario = await _context.Usuarios.FindAsync(entrada.UsuarioID);
                if (usuario == null)
                {
                    ModelState.AddModelError("UsuarioID", "El usuario no existe");
                }

                
                var bodega = await _context.Bodegas.FindAsync(entrada.BodegaID);
                if (bodega == null)
                {
                    ModelState.AddModelError("BodegaID", "La bodega no existe");
                }

                
                if (entrada.TipoMovimientoID == 0)
                {
                    ModelState.AddModelError("TipoMovimientoID", "Debe seleccionar un tipo de movimiento");
                }
                else
                {
                    
                    var tipoMovimiento = await _context.Set<TipoMovimiento>().FindAsync(entrada.TipoMovimientoID);
                    if (tipoMovimiento == null)
                    {
                        ModelState.AddModelError("TipoMovimientoID", "El tipo de movimiento seleccionado no existe");
                    }
                    else
                    {
                        
                        entrada.TipoMovimiento = tipoMovimiento.Codigo.Length > 20 
                            ? tipoMovimiento.Codigo.Substring(0, 20) 
                            : tipoMovimiento.Codigo;
                        
                        ModelState.Remove("TipoMovimiento");
                    }
                }

             
                if (entrada.Cantidad <= 0)
                {
                    ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        
                        var tipoMovimiento = await _context.Set<TipoMovimiento>().FindAsync(entrada.TipoMovimientoID);
                        if (tipoMovimiento == null)
                        {
                            ModelState.AddModelError("TipoMovimientoID", "El tipo de movimiento seleccionado no existe");
                            throw new InvalidOperationException("Tipo de movimiento no válido");
                        }

                        
                        var nuevaEntrada = new EntradaMercancia
                        {
                            UsuarioID = entrada.UsuarioID,
                            BodegaID = entrada.BodegaID,
                            ProductoID = entrada.ProductoID,
                            Cantidad = entrada.Cantidad,
                            TipoMovimientoID = entrada.TipoMovimientoID,
                            TipoMovimiento = tipoMovimiento.Codigo.Length > 20 
                                ? tipoMovimiento.Codigo.Substring(0, 20) 
                                : tipoMovimiento.Codigo,
                            Observaciones = entrada.Observaciones,
                            FechaEntrada = DateTime.UtcNow
                        };
                        
                        _context.EntradasMercancia.Add(nuevaEntrada);
                        await _context.SaveChangesAsync();

                      
                        await CrearMovimientoDesdeEntradaAsync(nuevaEntrada, tipoMovimiento);

                        TempData["Success"] = "Entrada de mercancía creada exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException dbEx)
                    {
                        
                        var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                        var errorMessage = $"Error al guardar en la base de datos: {innerException}";
                        
                       
                        if (innerException.Contains("FOREIGN KEY") || innerException.Contains("FK_"))
                        {
                            errorMessage = "Error: El producto, usuario o bodega seleccionado no existe en la base de datos. Por favor, verifique que todos los datos sean válidos.";
                        }
                        else if (innerException.Contains("NOT NULL") || innerException.Contains("NULL") || innerException.Contains("cannot be null"))
                        {
                            errorMessage = $"Error: Faltan campos requeridos. Detalles: {innerException}";
                        }
                        else if (innerException.Contains("String or binary data would be truncated") || innerException.Contains("truncated"))
                        {
                            errorMessage = "Error: Algún campo excede la longitud máxima permitida.";
                        }
                        else if (innerException.Contains("UNIQUE KEY") || innerException.Contains("duplicate"))
                        {
                            errorMessage = "Error: Ya existe un registro con estos datos.";
                        }
                        
                        ModelState.AddModelError("", errorMessage);
                        TempData["Error"] = errorMessage;
                    }
                }
                else
                {
                    
                    var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = "Por favor, corrija los siguientes errores: " + string.Join(", ", errores);
                }


                if (usuario == null)
                {
                    usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
                }
                var bodegaGeneral = await _context.Bodegas.FindAsync(1);

                ViewBag.UsuarioID = usuarioId.Value;
                ViewBag.UsuarioNombre = usuario?.Nombre ?? "Usuario";
                ViewBag.BodegaID = 1;
                ViewBag.BodegaNombre = bodegaGeneral?.Nombre ?? "Bodega General";
                
                var productos = await _context.Productos.Where(p => p.Activo).ToListAsync();
                ViewBag.Productos = new SelectList(productos, "ProductoID", "Nombre");
                
              
                var tiposMovimiento = await _context.Set<TipoMovimiento>()
                    .Where(tm => tm.Codigo == "Compra" || tm.Codigo == "Devolucion")
                    .ToListAsync();
                ViewBag.TiposMovimiento = new SelectList(tiposMovimiento, "TipoMovimientoID", "Codigo");

                return View(entrada);
            }
            catch (Exception ex)
            {
                
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Detalles: " + ex.InnerException.Message;
                }
                
                TempData["Error"] = "Error al crear la entrada de mercancía: " + errorMessage;
                
                
                var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
                if (usuarioId.HasValue)
                {
                    var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
                    var bodegaGeneral = await _context.Bodegas.FindAsync(1);

                    ViewBag.UsuarioID = usuarioId.Value;
                    ViewBag.UsuarioNombre = usuario?.Nombre ?? "Usuario";
                    ViewBag.BodegaID = 1;
                    ViewBag.BodegaNombre = bodegaGeneral?.Nombre ?? "Bodega General";
                    
                    var productos = await _context.Productos.Where(p => p.Activo).ToListAsync();
                    ViewBag.Productos = new SelectList(productos, "ProductoID", "Nombre");
                    
                   
                }

                return View(entrada);
            }
        }

        // EDITAR GET
       
        public async Task<IActionResult> Edit(int id)
        {
            var entrada = await _context.EntradasMercancia.FindAsync(id);
            if (entrada == null) return NotFound();

            CargarListas();
            return View(entrada);
        }

       
        // EDITAR POST
       
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

        
        // DELETE GET
      
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

        // DELETE POST
        
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

       
        // METODO PRUEBA : Crear Movimiento desde Entrada
     
        private async Task CrearMovimientoDesdeEntradaAsync(EntradaMercancia entrada, TipoMovimiento tipoMovimiento)
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("No se pudo obtener la cadena de conexión");
                }

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();
                try
                {
                    
                    var unidadId = await ObtenerOcrearUnidadPorDefectoAsync(connection, transaction);

                    
                    var referencia = $"ENT-{DateTime.UtcNow:yyyyMMddHHmmss}";

                    
                    var movimientoSql = @"
                        INSERT INTO inv.Movimientos (BodegaID, TipoMovimientoID, UsuarioID, FechaMovimientoUTC, Referencia, Observaciones)
                        VALUES (@BodegaID, @TipoMovimientoID, @UsuarioID, @FechaMovimientoUTC, @Referencia, @Observaciones);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    var movimientoId = await connection.QuerySingleAsync<int>(
                        movimientoSql,
                        new
                        {
                            entrada.BodegaID,
                            TipoMovimientoID = entrada.TipoMovimientoID,
                            entrada.UsuarioID,
                            FechaMovimientoUTC = entrada.FechaEntrada,
                            Referencia = referencia,
                            Observaciones = entrada.Observaciones
                        },
                        transaction);

                   
                    var detalleSql = @"
                        INSERT INTO inv.MovimientoDetalle (MovimientoID, ProductoID, UnidadID, Cantidad)
                        VALUES (@MovimientoID, @ProductoID, @UnidadID, @Cantidad);";

                    await connection.ExecuteAsync(
                        detalleSql,
                        new
                        {
                            MovimientoID = movimientoId,
                            entrada.ProductoID,
                            UnidadID = unidadId,
                            Cantidad = (decimal)entrada.Cantidad
                        },
                        transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
               
                System.Diagnostics.Debug.WriteLine($"Error al crear movimiento desde entrada: {ex.Message}");
            }
        }

        
        // METODO PRUEBA : Obtener o crear unidad por defecto
       
        private async Task<int> ObtenerOcrearUnidadPorDefectoAsync(SqlConnection connection, SqlTransaction transaction)
        {
         
            var unidad = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT UnidadID FROM inv.UnidadesMedida WHERE Nombre IN ('Unidad', 'Pieza')",
                transaction: transaction);

            if (unidad.HasValue)
            {
                return unidad.Value;
            }

            
            var nuevaUnidadId = await connection.QuerySingleAsync<int>(
                @"INSERT INTO inv.UnidadesMedida (Nombre) VALUES ('Unidad');
                  SELECT CAST(SCOPE_IDENTITY() as int);",
                transaction: transaction);

            return nuevaUnidadId;
        }
    }
}
