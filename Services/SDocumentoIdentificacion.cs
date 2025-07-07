using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Services
{
    public class SDocumentoIdentificacion : IDocIdentificacion
    {
        private readonly CarnetsFeriaContext _dbcontext;

        public SDocumentoIdentificacion(CarnetsFeriaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddUpdateAsync(DocumentoIdentificacion feriaidentificacion)
        {
            // Verifica si el ID es mayor que 0 para identificar si es una actualización
            if (feriaidentificacion.Id > 0)
            {
                // Buscar la participacion existente en la base de datos
                var existeIdentificacion = await _dbcontext.DocumentoIdentificacions.FindAsync(feriaidentificacion.Id);

                if (existeIdentificacion != null)
                {
                    // Actualizar las propiedades existentes
                    existeIdentificacion.TipoIdentificacion = feriaidentificacion.TipoIdentificacion;

                    // Marcar el espacio como modificado
                    _dbcontext.DocumentoIdentificacions.Update(existeIdentificacion);
                }
                else
                {
                    return false; // Si no se encontró el espacio, devolver false
                }
            }
            else
            {
                // Si no hay ID, se trata de un nuevo espacio, agregarlo
                _dbcontext.DocumentoIdentificacions.Add(feriaidentificacion);
            }

            // Guardar los cambios en la base de datos
            await _dbcontext.SaveChangesAsync();
            return true; // Retornar true si se ha agregado o actualizado correctamente
        }

        public async Task<bool> DeleteAsync(int id_feriaidentificacion)
        {
            var identificacion = await _dbcontext.DocumentoIdentificacions.FindAsync(id_feriaidentificacion);
            if (identificacion != null)
            {
                _dbcontext.DocumentoIdentificacions.Remove(identificacion);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false; // Devuelve false si no se encuentra
        }

        public async Task<List<DocumentoIdentificacion>> GetAllAsync()
        {
            return await _dbcontext.DocumentoIdentificacions
            .Where(fa => fa.Estatus == true) // Solo registros activos
            .ToListAsync();
        }

        public async Task<DocumentoIdentificacion> GetByIdAsync(int id_feriaIdentificacion)
        {
            try
            {
                var result = await _dbcontext.DocumentoIdentificacions
                    .FirstOrDefaultAsync(fa => fa.Id == id_feriaIdentificacion);

                if (result == null)
                {
                    // Manejar el caso donde no se encontró el objeto
                    throw new KeyNotFoundException($"No se encontró la participacion de feria con ID {id_feriaIdentificacion}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar la participacion de feria", ex);
            }
        }

        public async Task<MPaginatedResult<DocumentoIdentificacion>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            Console.WriteLine($"GetPaginatedAsync llamado - Página: {pageNumber}, Tamaño: {pageSize}, Búsqueda: '{searchTerm}'");

            var query = _dbcontext.DocumentoIdentificacions
                .Where(fa => fa.Estatus == true); // CORREGIDO: Mostrar solo registros activos (Estatus = true)

            // Debug: Contar total de registros antes del filtro
            var totalBeforeFilter = await _dbcontext.DocumentoIdentificacions.CountAsync();
            var totalActive = await query.CountAsync();
            Console.WriteLine($"Total registros en BD: {totalBeforeFilter}, Total activos: {totalActive}");

            // Filtro por el término de búsqueda
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(fa => fa.TipoIdentificacion.Contains(searchTerm));
                var totalAfterSearch = await query.CountAsync();
                Console.WriteLine($"Total después de búsqueda '{searchTerm}': {totalAfterSearch}");
            }

            // Ordenamiento basado en el campo descripcion_participacion
            query = sortAscending
                ? query.OrderBy(fa => fa.TipoIdentificacion).ThenBy(fa => fa.Id)
                : query.OrderByDescending(fa => fa.TipoIdentificacion).ThenByDescending(fa => fa.Id);

            var totalItems = await query.CountAsync();
            Console.WriteLine($"Total items para paginación: {totalItems}");

            // Aplicar paginación
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Items devueltos: {items.Count}");

            return new MPaginatedResult<DocumentoIdentificacion>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
