using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Services
{
    public class SPaticipacion : IParticipacion
    {
        private readonly CarnetsFeriaContext _dbcontext;

        public SPaticipacion(CarnetsFeriaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddUpdateAsync(Participacion feriaparticipacion)
        {
            // Verifica si el ID es mayor que 0 para identificar si es una actualización
            if (feriaparticipacion.Id > 0)
            {
                // Buscar la participacion existente en la base de datos
                var existeParticipacion = await _dbcontext.Participacions.FindAsync(feriaparticipacion.Id);

                if (existeParticipacion != null)
                {
                    // Actualizar las propiedades existentes
                    existeParticipacion.DescripcionParticipacion = feriaparticipacion.DescripcionParticipacion;

                    // Marcar el espacio como modificado
                    _dbcontext.Participacions.Update(existeParticipacion);
                }
                else
                {
                    return false; // Si no se encontró el espacio, devolver false
                }
            }
            else
            {
                // Si no hay ID, se trata de un nuevo espacio, agregarlo
                _dbcontext.Participacions.Add(feriaparticipacion);
            }

            // Guardar los cambios en la base de datos
            await _dbcontext.SaveChangesAsync();
            return true; // Retornar true si se ha agregado o actualizado correctamente
        }

        public async Task<bool> DeleteAsync(int id_feriaparticipaion)
        {
            var participacion = await _dbcontext.Participacions.FindAsync(id_feriaparticipaion);
            if (participacion != null)
            {
                _dbcontext.Participacions.Remove(participacion);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false; // Devuelve false si no se encuentra
        }

        public async Task<List<Participacion>> GetAllAsync()
        {
            return await _dbcontext.Participacions
                .Where(fa => fa.Estatus == true) // Solo registros activos
                .ToListAsync();
        }

        public async Task<Participacion> GetByIdAsync(int id_feriaparticipacion)
        {
            try
            {
                var result = await _dbcontext.Participacions
                    .FirstOrDefaultAsync(fa => fa.Id == id_feriaparticipacion);

                if (result == null)
                {
                    // Manejar el caso donde no se encontró el objeto
                    throw new KeyNotFoundException($"No se encontró la participacion de feria con ID {id_feriaparticipacion}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar la participacion de feria", ex);
            }
        }

        public async Task<MPaginatedResult<Participacion>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            Console.WriteLine($"GetPaginatedAsync llamado - Página: {pageNumber}, Tamaño: {pageSize}, Búsqueda: '{searchTerm}'");

            var query = _dbcontext.Participacions
                .Where(fa => fa.Estatus == true); // CORREGIDO: Mostrar solo registros activos (Estatus = true)

            // Debug: Contar total de registros antes del filtro
            var totalBeforeFilter = await _dbcontext.Participacions.CountAsync();
            var totalActive = await query.CountAsync();
            Console.WriteLine($"Total registros en BD: {totalBeforeFilter}, Total activos: {totalActive}");

            // Filtro por el término de búsqueda
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(fa => fa.DescripcionParticipacion.Contains(searchTerm));
                var totalAfterSearch = await query.CountAsync();
                Console.WriteLine($"Total después de búsqueda '{searchTerm}': {totalAfterSearch}");
            }

            // Ordenamiento basado en el campo descripcion_participacion
            query = sortAscending
                ? query.OrderBy(fa => fa.DescripcionParticipacion).ThenBy(fa => fa.Id)
                : query.OrderByDescending(fa => fa.DescripcionParticipacion).ThenByDescending(fa => fa.Id);

            var totalItems = await query.CountAsync();
            Console.WriteLine($"Total items para paginación: {totalItems}");

            // Aplicar paginación
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Items devueltos: {items.Count}");

            return new MPaginatedResult<Participacion>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}