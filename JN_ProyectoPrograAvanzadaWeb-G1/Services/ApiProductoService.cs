using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiProductoService : IApiProductoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiProductoService> _logger;

        public ApiProductoService(IHttpClientFactory httpClientFactory, ILogger<ApiProductoService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<ProductoDto>> GetAllAsync(bool? activo = null)
        {
            try
            {
                var url = activo.HasValue ? $"api/productos?activo={activo.Value}" : "api/productos";
                var response = await _httpClient.GetFromJsonAsync<List<ProductoDto>>(url);
                return response ?? new List<ProductoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return new List<ProductoDto>();
            }
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductoDto>($"api/productos/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return null;
            }
        }

        public async Task<ProductoDto?> GetBySkuAsync(string sku)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductoDto>($"api/productos/sku/{Uri.EscapeDataString(sku)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto por SKU {Sku}", sku);
                return null;
            }
        }

        public async Task<List<ProductoDto>> SearchAsync(string term)
        {
            try
            {
                var url = $"api/productos/search?term={Uri.EscapeDataString(term)}";
                var response = await _httpClient.GetFromJsonAsync<List<ProductoDto>>(url);
                return response ?? new List<ProductoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return new List<ProductoDto>();
            }
        }

        public async Task<int> CreateAsync(CrearProductoDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/productos", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (result.TryGetProperty("id", out var id))
                    {
                        return id.GetInt32();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, CrearProductoDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/productos/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                return false;
            }
        }

        public async Task<bool> ToggleActivoAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/productos/{id}/toggle-activo", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de producto {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/productos/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var producto = await GetByIdAsync(id);
                return producto != null;
            }
            catch
            {
                return false;
            }
        }
    }
}

