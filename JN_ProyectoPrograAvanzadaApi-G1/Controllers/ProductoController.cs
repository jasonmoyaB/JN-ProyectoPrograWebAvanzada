using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductoController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        [Route("Obtener")]
        public IActionResult Obtener()
        {
            using (var context= new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                var productos = new DynamicParameters();
                var result = context.Query<ProductoModel>("inv.sp_Producto_GetAll", productos).ToList();
                return Ok(result);
            }
                
        }
    }
}
