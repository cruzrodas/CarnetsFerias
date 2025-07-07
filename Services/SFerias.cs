using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Services
{
    public class SFerias : IFeria
    {
        private readonly CarnetsFeriaContext _dbcontext;

        public SFerias(CarnetsFeriaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddUpdateAsync(Ferium feriaevento)
        {
            // Verifica si el ID es mayor que 0 para identificar si es una actualización
            if (feriaevento.Id > 0)
            {
                // Buscar la participacion existente en la base de datos
                var existeFeria = await _dbcontext.Feria.FindAsync(feriaevento.Id);

                if (existeFeria != null)
                {
                    // Actualizar las propiedades existentes
                    existeFeria.DescripcionFeria = feriaevento.DescripcionFeria;
                    existeFeria.FechaInicio = feriaevento.FechaInicio;
                    existeFeria.FechaFin = feriaevento.FechaFin;
                    existeFeria.Activa = feriaevento.Activa;

                    // Marcar el espacio como modificado
                    _dbcontext.Feria.Update(existeFeria);
                }
                else
                {
                    return false; // Si no se encontró el espacio, devolver false
                }
            }
            else
            {
                // Si no hay ID, se trata de un nuevo espacio, agregarlo
                _dbcontext.Feria.Add(feriaevento);
            }

            // Guardar los cambios en la base de datos
            await _dbcontext.SaveChangesAsync();
            return true; // Retornar true si se ha agregado o actualizado correctamente
        }

        public async  Task<bool> DeleteAsync(int id_feriaievento)
        {
            var feria = await _dbcontext.Feria.FindAsync(id_feriaievento);
            if (feria != null)
            {
                _dbcontext.Feria.Remove(feria);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false; // Devuelve false si no se encuentra
        }

        public async  Task<List<Ferium>> GetAllAsync()
        {
            return await _dbcontext.Feria
            .Where(fa => fa.Estatus == true) // Solo registros activos
            .ToListAsync();
        }

        public async Task<Ferium> GetByIdAsync(int id_feriaevento)
        {
            try
            {
                var result = await _dbcontext.Feria
                    .FirstOrDefaultAsync(fa => fa.Id == id_feriaevento);

                if (result == null)
                {
                    // Manejar el caso donde no se encontró el objeto
                    throw new KeyNotFoundException($"No se encontró la feria con ID {id_feriaevento}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar la feria", ex);
            }
        }

        public async Task<MPaginatedResult<Ferium>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            Console.WriteLine($"GetPaginatedAsync llamado - Página: {pageNumber}, Tamaño: {pageSize}, Búsqueda: '{searchTerm}'");

            var query = _dbcontext.Feria
                .Where(fa => fa.Estatus == true); // CORREGIDO: Mostrar solo registros activos (Estatus = true)

            // Debug: Contar total de registros antes del filtro
            var totalBeforeFilter = await _dbcontext.Feria.CountAsync();
            var totalActive = await query.CountAsync();
            Console.WriteLine($"Total registros en BD: {totalBeforeFilter}, Total activos: {totalActive}");

            // Filtro por el término de búsqueda
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(fa => fa.DescripcionFeria.Contains(searchTerm));
                var totalAfterSearch = await query.CountAsync();
                Console.WriteLine($"Total después de búsqueda '{searchTerm}': {totalAfterSearch}");
            }

            // Ordenamiento basado en el campo descripcion_participacion
            query = sortAscending
                ? query.OrderBy(fa => fa.DescripcionFeria).ThenBy(fa => fa.Id)
                : query.OrderByDescending(fa => fa.DescripcionFeria).ThenByDescending(fa => fa.Id);

            var totalItems = await query.CountAsync();
            Console.WriteLine($"Total items para paginación: {totalItems}");

            // Aplicar paginación
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Items devueltos: {items.Count}");

            return new MPaginatedResult<Ferium>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
