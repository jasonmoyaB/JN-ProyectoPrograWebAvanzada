using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Bodegas;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public class BodegaService : IBodegaService
    {
        private readonly IBodegaRepository _bodegaRepository;

        public BodegaService(IBodegaRepository bodegaRepository)
        {
            _bodegaRepository = bodegaRepository;
        }

        public async Task<List<BodegaDto>> GetAllAsync(bool? activo = null)
        {
            var bodegas = await _bodegaRepository.GetAllAsync(activo);
            return bodegas.Select(b => new BodegaDto
            {
                BodegaID = b.BodegaID,
                Nombre = b.Nombre,
                Ubicacion = b.Ubicacion,
                Activo = b.Activo,
                FechaCreacion = b.FechaCreacion
            }).ToList();
        }

        public async Task<BodegaDto?> GetByIdAsync(int bodegaId)
        {
            var bodega = await _bodegaRepository.GetByIdAsync(bodegaId);
            if (bodega == null) return null;

            return new BodegaDto
            {
                BodegaID = bodega.BodegaID,
                Nombre = bodega.Nombre,
                Ubicacion = bodega.Ubicacion,
                Activo = bodega.Activo,
                FechaCreacion = bodega.FechaCreacion
            };
        }

        public async Task<int> CreateAsync(CrearBodegaDto dto)
        {
            var bodega = new Bodega
            {
                Nombre = dto.Nombre,
                Ubicacion = dto.Ubicacion,
                Activo = true
            };

            return await _bodegaRepository.CreateAsync(bodega);
        }

        public async Task<bool> UpdateAsync(int bodegaId, CrearBodegaDto dto)
        {
            var bodega = await _bodegaRepository.GetByIdAsync(bodegaId);
            if (bodega == null) return false;

            bodega.Nombre = dto.Nombre;
            bodega.Ubicacion = dto.Ubicacion;

            return await _bodegaRepository.UpdateAsync(bodega);
        }

        public async Task<bool> ToggleActivoAsync(int bodegaId)
        {
            return await _bodegaRepository.ToggleActivoAsync(bodegaId);
        }

        public async Task<bool> ExistsAsync(int bodegaId)
        {
            return await _bodegaRepository.ExistsAsync(bodegaId);
        }

        public async Task<bool> DeleteAsync(int bodegaId)
        {
            return await _bodegaRepository.DeleteAsync(bodegaId);
        }
    }
}

