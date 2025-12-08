using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiInventarioService : IApiInventarioService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiInventarioService> _logger;

        public ApiInventarioService(IHttpClientFactory httpClientFactory, ILogger<ApiInventarioService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<SaldoInventarioDto>> GetSaldoByBodegaAsync(int bodegaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SaldoInventarioDto>>($"api/inventario/bodega/{bodegaId}");
                return response ?? new List<SaldoInventarioDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inventario por bodega {BodegaId}", bodegaId);
                return new List<SaldoInventarioDto>();
            }
        }

        public async Task<SaldoInventarioDto?> GetSaldoByBodegaAndProductoAsync(int bodegaId, int productoId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<SaldoInventarioDto>($"api/inventario/bodega/{bodegaId}/producto/{productoId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener saldo por bodega {BodegaId} y producto {ProductoId}", bodegaId, productoId);
                return null;
            }
        }

        public async Task<List<SaldoInventarioDto>> GetSaldosConAlertaStockBajoByBodegaAsync(int bodegaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SaldoInventarioDto>>($"api/inventario/bodega/{bodegaId}/stock-bajo");
                return response ?? new List<SaldoInventarioDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener stock bajo por bodega {BodegaId}", bodegaId);
                return new List<SaldoInventarioDto>();
            }
        }

        public async Task<int> GetTotalProductosByBodegaAsync(int bodegaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"api/inventario/bodega/{bodegaId}/resumen");
                if (response.TryGetProperty("totalProductos", out var total))
                {
                    return total.GetInt32();
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de productos por bodega {BodegaId}", bodegaId);
                return 0;
            }
        }

        public async Task<int> GetTotalStockBajoByBodegaAsync(int bodegaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"api/inventario/bodega/{bodegaId}/resumen");
                if (response.TryGetProperty("totalStockBajo", out var total))
                {
                    return total.GetInt32();
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de stock bajo por bodega {BodegaId}", bodegaId);
                return 0;
            }
        }
    }
}

