using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Services
{
    public class SAreasdeFeria : IAreasdeFeria
    {
        private readonly CarnetsFeriaContext _dbcontext;

        public SAreasdeFeria(CarnetsFeriaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddUpdateAsync(FeriaArea areasdeferia)
        {
            try
            {
                // Verifica si el ID es mayor que 0 para identificar si es una actualización
                if (areasdeferia.Id > 0)
                {
                    // Buscar el área existente en la base de datos
                    var existeareaferia = await _dbcontext.FeriaAreas.FindAsync(areasdeferia.Id);

                    if (existeareaferia != null)
                    {
                        // Actualizar las propiedades existentes
                        existeareaferia.DescripcionArea = areasdeferia.DescripcionArea;
                        existeareaferia.CantidadStand = areasdeferia.CantidadStand;
                        existeareaferia.IdFeria = areasdeferia.IdFeria;
                        existeareaferia.IdEspacio = areasdeferia.IdEspacio;
                        existeareaferia.Estatus = areasdeferia.Estatus; // Activa/Inactiva
                        existeareaferia.Eliminada = areasdeferia.Eliminada; // Visible/No visible

                        // OPCIONAL: Agregar campo de fecha de modificación
                        // existeareaferia.FechaModificacion = DateTime.Now;

                        _dbcontext.FeriaAreas.Update(existeareaferia);
                    }
                    else
                    {
                        return false; // Si no se encontró el área, devolver false
                    }
                }
                else
                {
                    // Si no hay ID, se trata de una nueva área, agregarla
                    // Por defecto: Eliminada = false (visible), Estatus = true (activa)
                    areasdeferia.Eliminada = false; // Visible en listas
                    areasdeferia.Estatus = true;    // Activa por defecto

                    // OPCIONAL: Agregar fecha de creación
                    // areasdeferia.FechaCreacion = DateTime.Now;
                    _dbcontext.FeriaAreas.Add(areasdeferia);
                }

                // Guardar los cambios en la base de datos
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el área de feria", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id_areasdeferia)
        {
            try
            {
                var areasferia = await _dbcontext.FeriaAreas.FindAsync(id_areasdeferia);

                if (areasferia != null)
                {
                    // Eliminación lógica: Marcar como eliminada (no visible en listas)
                    areasferia.Eliminada = true; // Ocultar de las listas
                                                 // El Estatus se mantiene como estaba, solo se oculta

                    _dbcontext.FeriaAreas.Update(areasferia);
                    await _dbcontext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el área de feria", ex);
            }
        }

        public async Task<List<FeriaArea>> GetAllAsync()
        {
            try
            {
                return await _dbcontext.FeriaAreas
                    .Include(fa => fa.IdEspacioNavigation) // Incluir datos del espacio
                    .Include(fa => fa.IdFeriaNavigation)   // Incluir datos de la feria
                    .Where(ep => ep.Eliminada == false)    // Solo registros NO eliminados (visibles)
                    .OrderBy(fa => fa.DescripcionArea)     // Ordenar alfabéticamente
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las áreas de feria", ex);
            }
        }

        public async Task<FeriaArea> GetByIdAsync(int id_areasdeferia)
        {
            try
            {
                var result = await _dbcontext.FeriaAreas
                    .Include(fa => fa.IdEspacioNavigation) // Incluir navegación
                    .Include(fa => fa.IdFeriaNavigation)   // Incluir navegación
                    .FirstOrDefaultAsync(ep => ep.Id == id_areasdeferia && ep.Eliminada == false);

                if (result == null)
                {
                    throw new KeyNotFoundException($"No se encontró el área de feria con ID {id_areasdeferia}");
                }

                return result;
            }
            catch (KeyNotFoundException)
            {
                throw; // Re-lanzar la excepción específica
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar el área de feria", ex);
            }
        }

        // Método adicional para obtener solo áreas activas (Estatus = true y Eliminada = false)
        public async Task<List<FeriaArea>> GetActiveAsync()
        {
            try
            {
                return await _dbcontext.FeriaAreas
                    .Include(fa => fa.IdEspacioNavigation)
                    .Include(fa => fa.IdFeriaNavigation)
                    .Where(ep => ep.Eliminada == false && ep.Estatus == true) // Solo visibles Y activas
                    .OrderBy(fa => fa.DescripcionArea)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las áreas de feria activas", ex);
            }
        }

        public async Task<List<EspacioParque>> GetEspaciosAsync()
        {
            try
            {
                return await _dbcontext.EspacioParques
                    .Where(f => f.FeriaUso == true &&
                               f.Arrendamineto == true &&
                               f.Estatus == true) // Combinar condiciones WHERE
                    .OrderBy(ep => ep.DescripcionEspacio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los espacios disponibles", ex);
            }
        }

        public async Task<List<Ferium>> GetFeriaAsync()
        {
            try
            {
                return await _dbcontext.Feria
                    .Where(f => f.Activa == true && f.Estatus == true) // Combinar condiciones
                    .OrderBy(f => f.DescripcionFeria)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ferias activas", ex);
            }
        }

        public async Task<MPaginatedResult<FeriaArea>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            try
            {
                // Validaciones de entrada
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Límite máximo

                Console.WriteLine($"GetPaginatedAsync llamado - Página: {pageNumber}, Tamaño: {pageSize}, Búsqueda: '{searchTerm}'");

                // Construir la consulta base
                var query = _dbcontext.FeriaAreas
                    .Include(fa => fa.IdEspacioNavigation) // Incluir navegación para búsqueda
                    .Include(fa => fa.IdFeriaNavigation)   // Incluir navegación
                    .Where(ep => ep.Eliminada == false);   // Mostrar solo registros NO eliminados (visibles)

                // Aplicar filtro de búsqueda si existe
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchTermLower = searchTerm.ToLower();
                    query = query.Where(ep =>
                        ep.DescripcionArea.ToLower().Contains(searchTermLower) ||
                        (ep.IdEspacioNavigation != null &&
                         ep.IdEspacioNavigation.DescripcionEspacio.ToLower().Contains(searchTermLower)));
                }

                // Aplicar ordenamiento
                query = sortAscending
                    ? query.OrderBy(ep => ep.DescripcionArea).ThenBy(ep => ep.Id)
                    : query.OrderByDescending(ep => ep.DescripcionArea).ThenByDescending(ep => ep.Id);

                // OPTIMIZACIÓN: Una sola operación Count y una sola consulta para los datos
                var totalItems = await query.CountAsync();
                Console.WriteLine($"Total items encontrados: {totalItems}");

                // Aplicar paginación
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Console.WriteLine($"Items devueltos: {items.Count}");

                return new MPaginatedResult<FeriaArea>
                {
                    Items = items,
                    TotalCount = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos paginados", ex);
            }
        }

        // Método adicional para obtener datos paginados solo de áreas activas
        public async Task<MPaginatedResult<FeriaArea>> GetPaginatedActiveAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            try
            {
                // Validaciones de entrada
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _dbcontext.FeriaAreas
                    .Include(fa => fa.IdEspacioNavigation)
                    .Include(fa => fa.IdFeriaNavigation)
                    .Where(ep => ep.Eliminada == false && ep.Estatus == true); // Solo visibles Y activas

                // Filtro por término de búsqueda
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchTermLower = searchTerm.ToLower();
                    query = query.Where(ep =>
                        ep.DescripcionArea.ToLower().Contains(searchTermLower) ||
                        (ep.IdEspacioNavigation != null &&
                         ep.IdEspacioNavigation.DescripcionEspacio.ToLower().Contains(searchTermLower)));
                }

                // Ordenamiento
                query = sortAscending
                    ? query.OrderBy(ep => ep.DescripcionArea).ThenBy(ep => ep.Id)
                    : query.OrderByDescending(ep => ep.DescripcionArea).ThenByDescending(ep => ep.Id);

                var totalItems = await query.CountAsync();

                // Aplicar paginación
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new MPaginatedResult<FeriaArea>
                {
                    Items = items,
                    TotalCount = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos paginados de áreas activas", ex);
            }
        }
    }
}