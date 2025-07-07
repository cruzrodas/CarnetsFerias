using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Services
{
    public class SEspacioParque : IEspacioParque
    {
        private readonly CarnetsFeriaContext _dbcontext;

        public SEspacioParque(CarnetsFeriaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddUpdateAsync(EspacioParque espacioparque)
        {
            // Verifica si el ID es mayor que 0 para identificar si es una actualización
            if (espacioparque.Id > 0)
            {
                // Buscar el espacio existente en la base de datos
                var existeespacio = await _dbcontext.EspacioParques.FindAsync(espacioparque.Id);

                if (existeespacio != null)
                {
                    // Actualizar las propiedades existentes
                    existeespacio.DescripcionEspacio = espacioparque.DescripcionEspacio;
                    existeespacio.CapacidadPersonas = espacioparque.CapacidadPersonas;
                    existeespacio.CapacidadVehiculos = espacioparque.CapacidadVehiculos;
                    existeespacio.MetrosAncho = espacioparque.MetrosAncho;
                    existeespacio.MetrosLargo = espacioparque.MetrosLargo;
                    existeespacio.TotalMetros = espacioparque.TotalMetros;
                    existeespacio.Informacion = espacioparque.Informacion;
                    existeespacio.FeriaUso = espacioparque.FeriaUso;
                    existeespacio.Estatus = espacioparque.Estatus;
                    existeespacio.Arrendamineto = espacioparque.Arrendamineto;

                    // ✅ CORREGIDO: Marcar la entidad existente como modificada
                    _dbcontext.EspacioParques.Update(existeespacio);
                }
                else
                {
                    return false; // Si no se encontró el espacio, devolver false
                }
            }
            else
            {
                // Si no hay ID, se trata de un nuevo espacio, agregarlo
                _dbcontext.EspacioParques.Add(espacioparque);
            }

            // Guardar los cambios en la base de datos
            await _dbcontext.SaveChangesAsync();
            return true; // Retornar true si se ha agregado o actualizado correctamente
        }

        public async Task<bool> DeleteAsync(int id_espacioparque)
        {
            // ✅ CORREGIDO: Sintaxis del FindAsync
            var espacioparque = await _dbcontext.EspacioParques.FindAsync(id_espacioparque);

            if (espacioparque != null)
            {
                _dbcontext.EspacioParques.Remove(espacioparque);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false; // Devuelve false si no se encuentra
        }

        public async Task<List<EspacioParque>> GetAllAsync()
        {
            return await _dbcontext.EspacioParques
                .Where(ep => ep.Estatus == true) // Solo registros activos
                .ToListAsync();
        }

        public async Task<EspacioParque> GetByIdAsync(int id_espacioparque)
        {
            try
            {
                var result = await _dbcontext.EspacioParques
                    .FirstOrDefaultAsync(ep => ep.Id == id_espacioparque);

                if (result == null)
                {
                    // Manejar el caso donde no se encontró el objeto
                    throw new KeyNotFoundException($"No se encontró el espacio parque con ID {id_espacioparque}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar el espacio parque", ex);
            }
        }

        public async Task<MPaginatedResult<EspacioParque>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            Console.WriteLine($"GetPaginatedAsync llamado - Página: {pageNumber}, Tamaño: {pageSize}, Búsqueda: '{searchTerm}'");

            var query = _dbcontext.EspacioParques
                .Where(ep => ep.Estatus == true); // Mostrar solo registros activos

            // Debug: Contar total de registros antes del filtro
            var totalBeforeFilter = await _dbcontext.EspacioParques.CountAsync();
            var totalActive = await query.CountAsync();
            Console.WriteLine($"Total registros en BD: {totalBeforeFilter}, Total activos: {totalActive}");

            // Filtro por el término de búsqueda
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(ep => ep.DescripcionEspacio.Contains(searchTerm) ||
                                         ep.Informacion.Contains(searchTerm));
                var totalAfterSearch = await query.CountAsync();
                Console.WriteLine($"Total después de búsqueda '{searchTerm}': {totalAfterSearch}");
            }

            // Ordenamiento basado en el campo DescripcionEspacio
            query = sortAscending
                ? query.OrderBy(ep => ep.DescripcionEspacio).ThenBy(ep => ep.Id)
                : query.OrderByDescending(ep => ep.DescripcionEspacio).ThenByDescending(ep => ep.Id);

            var totalItems = await query.CountAsync();
            Console.WriteLine($"Total items para paginación: {totalItems}");

            // Aplicar paginación
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Items devueltos: {items.Count}");

            return new MPaginatedResult<EspacioParque>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}