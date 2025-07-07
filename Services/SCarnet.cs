using CarnetsdeFeria.Components.Pages;
using CarnetsdeFeria.Interfaces;
using CarnetsdeFeria.Models;
using Microsoft.EntityFrameworkCore;

namespace CarnetsdeFeria.Services
{
    public class SCarnet : ICarnet
    {
        private readonly CarnetsFeriaContext _dbcontext;
        private readonly IWebHostEnvironment _environment;

        public SCarnet(CarnetsFeriaContext dbcontext, IWebHostEnvironment environment)
        {
            _dbcontext = dbcontext;
            _environment = environment;
        }

        // Método para guardar imagen desde base64
        public async Task<string> SaveImageFromBase64Async(string base64Image, string fileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                    return null;

                // Remover el prefijo data:image/jpeg;base64, si existe
                var base64Data = base64Image.Contains(",") ? base64Image.Split(',')[1] : base64Image;

                // Convertir de base64 a bytes
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                // Crear nombre único para el archivo
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"carnet_{Guid.NewGuid():N}.jpg";
                }

                // Crear directorio si no existe
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "carnets");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Guardar archivo
                var filePath = Path.Combine(uploadsPath, fileName);
                await File.WriteAllBytesAsync(filePath, imageBytes);

                // Retornar la URL relativa
                return $"/uploads/carnets/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar la imagen: {ex.Message}", ex);
            }
        }

        // Método para eliminar imagen física
        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith("/uploads/"))
                    return true; // No es una imagen local, no necesita eliminación

                var filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log del error pero no lanzar excepción para no interrumpir el flujo principal
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddUpdateAsync(FeriaCarnet carnetFeria)
        {
            try
            {
                string oldImageUrl = null;

                // Verifica si el ID es mayor que 0 para identificar si es una actualización
                if (carnetFeria.Id > 0)
                {
                    // Buscar el carnet existente en la base de datos
                    var existecarnet = await _dbcontext.FeriaCarnets.FindAsync(carnetFeria.Id);

                    if (existecarnet != null)
                    {
                        // Obtener el ID de la feria actual del carnet existente
                        var feriaArea = await _dbcontext.FeriaAreas.FindAsync(carnetFeria.IdFeriaArea);
                        if (feriaArea != null)
                        {
                            // Validar que no exista otro carnet con el mismo DPI en la misma feria
                            var existeCarnetEnFeria = await ExistsCarnetByIdentificationInFeriaAsync(
                                carnetFeria.IdDocIdentificacion,
                                carnetFeria.NoIdenitficacion,
                                feriaArea.IdFeria,
                                carnetFeria.Id);

                            if (existeCarnetEnFeria)
                            {
                                throw new Exception("Ya existe un carnet registrado con ese número de identificación en esta feria.");
                            }
                        }

                        // Guardar la URL de la imagen anterior para eliminarla si se cambió
                        oldImageUrl = existecarnet.Foto;

                        // Si hay una nueva imagen en base64, procesarla
                        if (!string.IsNullOrEmpty(carnetFeria.Foto) && carnetFeria.Foto.StartsWith("data:image"))
                        {
                            // Eliminar imagen anterior si existe
                            if (!string.IsNullOrEmpty(oldImageUrl))
                            {
                                await DeleteImageAsync(oldImageUrl);
                            }

                            // Guardar nueva imagen
                            carnetFeria.Foto = await SaveImageFromBase64Async(carnetFeria.Foto);
                        }

                        // Actualizar las propiedades existentes
                        existecarnet.Nombre = carnetFeria.Nombre;
                        existecarnet.Apellido = carnetFeria.Apellido;
                        existecarnet.IdParticipacion = carnetFeria.IdParticipacion;
                        existecarnet.IdDocIdentificacion = carnetFeria.IdDocIdentificacion;
                        existecarnet.NoIdenitficacion = carnetFeria.NoIdenitficacion;
                        existecarnet.NoBoleta = carnetFeria.NoBoleta;
                        existecarnet.BoletaCortesia = carnetFeria.BoletaCortesia;
                        existecarnet.Foto = carnetFeria.Foto;
                        existecarnet.FechaModificacion = DateOnly.FromDateTime(DateTime.Now);
                        existecarnet.Impresion = carnetFeria.Impresion;
                        existecarnet.IdFeriaArea = carnetFeria.IdFeriaArea;
                        existecarnet.NoStand = carnetFeria.NoStand;
                        existecarnet.Estatus = carnetFeria.Estatus;

                        _dbcontext.FeriaCarnets.Update(existecarnet);
                    }
                    else
                    {
                        return false; // Si no se encontró el carnet, devolver false
                    }
                }
                else
                {
                    // Para nuevos carnets, validar que no exista en la misma feria
                    var feriaArea = await _dbcontext.FeriaAreas.FindAsync(carnetFeria.IdFeriaArea);
                    if (feriaArea != null)
                    {
                        var existeCarnetEnFeria = await ExistsCarnetByIdentificationInFeriaAsync(
                            carnetFeria.IdDocIdentificacion,
                            carnetFeria.NoIdenitficacion,
                            feriaArea.IdFeria);

                        if (existeCarnetEnFeria)
                        {
                            throw new Exception("Ya existe un carnet registrado con ese número de identificación en esta feria.");
                        }
                    }

                    // Si hay una imagen en base64, procesarla
                    if (!string.IsNullOrEmpty(carnetFeria.Foto) && carnetFeria.Foto.StartsWith("data:image"))
                    {
                        carnetFeria.Foto = await SaveImageFromBase64Async(carnetFeria.Foto);
                    }

                    // Si no hay ID, se trata de un nuevo carnet
                    carnetFeria.Estatus = true; // Activo por defecto
                    carnetFeria.Impresion = false; // No impreso por defecto
                    carnetFeria.BoletaCortesia = carnetFeria.BoletaCortesia; // Mantener el valor enviado
                    carnetFeria.FechaCreacion = DateOnly.FromDateTime(DateTime.Now);
                    carnetFeria.FechaModificacion = null; // No hay modificación en creación

                    _dbcontext.FeriaCarnets.Add(carnetFeria);
                }

                // Guardar los cambios en la base de datos
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el carnet de feria", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id_carnetferia)
        {
            try
            {
                var feriacarnet = await _dbcontext.FeriaCarnets.FindAsync(id_carnetferia);
                if (feriacarnet != null)
                {
                    // Eliminar imagen física si existe
                    if (!string.IsNullOrEmpty(feriacarnet.Foto))
                    {
                        await DeleteImageAsync(feriacarnet.Foto);
                    }

                    _dbcontext.FeriaCarnets.Remove(feriacarnet);
                    await _dbcontext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el carnet", ex);
            }
        }

        public async Task<List<FeriaCarnet>> GetAllAsync()
        {
            try
            {
                return await _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdParticipacionNavigation)
                    .Include(fc => fc.IdDocIdentificacionNavigation)
                    .Include(fc => fc.IdFeriaAreaNavigation)
                        .ThenInclude(fa => fa.IdFeriaNavigation)
                    .Where(fa => fa.Estatus == true) // Solo registros activos
                    .OrderBy(fc => fc.Nombre)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener todos los carnets", ex);
            }
        }

        public async Task<List<FeriaArea>> GetAreaAsync()
        {
            try
            {
                return await _dbcontext.FeriaAreas
                    .Include(fa => fa.IdFeriaNavigation) // Incluir información de la feria
                    .Where(f => f.Estatus == true)
                    .OrderBy(f => f.DescripcionArea)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las áreas activas", ex);
            }
        }

        public async Task<FeriaCarnet> GetByIdAsync(int id_carnetFeria)
        {
            try
            {
                var result = await _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdParticipacionNavigation)
                    .Include(fc => fc.IdDocIdentificacionNavigation)
                    .Include(fc => fc.IdFeriaAreaNavigation)
                        .ThenInclude(fa => fa.IdFeriaNavigation)
                    .FirstOrDefaultAsync(fa => fa.Id == id_carnetFeria);

                if (result == null)
                {
                    throw new KeyNotFoundException($"No se encontró el carnet con ID {id_carnetFeria}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar el carnet", ex);
            }
        }

        public async Task<List<DocumentoIdentificacion>> GetIdentificacionAsync()
        {
            try
            {
                return await _dbcontext.DocumentoIdentificacions
                    .Where(f => f.Estatus == true)
                    .OrderBy(f => f.TipoIdentificacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los tipos de identificación activos", ex);
            }
        }

        public async Task<MPaginatedResult<FeriaCarnet>> GetPaginatedActiveAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            try
            {
                // Validaciones de entrada
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdParticipacionNavigation)
                    .Include(fc => fc.IdDocIdentificacionNavigation)
                    .Include(fc => fc.IdFeriaAreaNavigation)
                        .ThenInclude(fa => fa.IdFeriaNavigation)
                    .Where(ep => ep.Estatus == true); // Solo activos

                // Filtro por término de búsqueda
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchTermLower = searchTerm.ToLower();
                    query = query.Where(ep =>
                        ep.Nombre.ToLower().Contains(searchTermLower) ||
                        ep.Apellido.ToLower().Contains(searchTermLower) ||
                        (ep.IdFeriaAreaNavigation != null &&
                         ep.IdFeriaAreaNavigation.DescripcionArea.ToLower().Contains(searchTermLower)) ||
                        (ep.IdParticipacionNavigation != null &&
                         ep.IdParticipacionNavigation.DescripcionParticipacion.ToLower().Contains(searchTermLower)));
                }

                // Ordenamiento
                query = sortAscending
                    ? query.OrderBy(ep => ep.Nombre).ThenBy(ep => ep.Apellido)
                    : query.OrderByDescending(ep => ep.Nombre).ThenByDescending(ep => ep.Apellido);

                var totalItems = await query.CountAsync();

                // Aplicar paginación
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new MPaginatedResult<FeriaCarnet>
                {
                    Items = items,
                    TotalCount = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos paginados de carnets activos", ex);
            }
        }

        public async Task<MPaginatedResult<FeriaCarnet>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true)
        {
            try
            {
                // Validaciones de entrada
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // Construir la consulta base una sola vez
                var baseQuery = _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdParticipacionNavigation)
                    .Include(fc => fc.IdDocIdentificacionNavigation)
                    .Include(fc => fc.IdFeriaAreaNavigation)
                        .ThenInclude(fa => fa.IdFeriaNavigation)
                    .Where(fa => fa.Estatus == true); // Solo registros activos

                // Aplicar filtro de búsqueda si existe
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchTermLower = searchTerm.ToLower();
                    baseQuery = baseQuery.Where(fa =>
                        fa.Nombre.ToLower().Contains(searchTermLower) ||
                        fa.Apellido.ToLower().Contains(searchTermLower) ||
                        (fa.IdFeriaAreaNavigation != null &&
                         fa.IdFeriaAreaNavigation.DescripcionArea.ToLower().Contains(searchTermLower)) ||
                        (fa.IdParticipacionNavigation != null &&
                         fa.IdParticipacionNavigation.DescripcionParticipacion.ToLower().Contains(searchTermLower)));
                }

                // Aplicar ordenamiento
                var orderedQuery = sortAscending
                    ? baseQuery.OrderBy(fa => fa.Nombre).ThenBy(fa => fa.Apellido)
                    : baseQuery.OrderByDescending(fa => fa.Nombre).ThenByDescending(fa => fa.Apellido);

                // Ejecutar ambas consultas de forma secuencial para evitar concurrencia
                var totalItems = await orderedQuery.CountAsync();

                var items = await orderedQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new MPaginatedResult<FeriaCarnet>
                {
                    Items = items,
                    TotalCount = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos paginados de carnets", ex);
            }
        }

        public async Task<List<Models.Participacion>> GetParticipacionAsync()
        {
            try
            {
                return await _dbcontext.Participacions
                    .Where(f => f.Estatus == true)
                    .OrderBy(f => f.DescripcionParticipacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las participaciones activas", ex);
            }
        }

        // MÉTODO ACTUALIZADO: Verificar existencia de carnet por identificación en la misma feria
        public async Task<bool> ExistsCarnetByIdentificationInFeriaAsync(int idDocIdentificacion, long noIdentificacion, int? idFeria, int? excludeId = null)
        {
            try
            {
                // Si no se proporciona idFeria, no se puede hacer la validación
                if (!idFeria.HasValue)
                    return false;

                var query = _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdFeriaAreaNavigation)
                    .Where(fc => fc.IdDocIdentificacion == idDocIdentificacion &&
                                fc.NoIdenitficacion == noIdentificacion &&
                                fc.Estatus == true &&
                                fc.IdFeriaAreaNavigation.IdFeria == idFeria.Value);

                // Si se proporciona excludeId, excluir ese registro (útil para ediciones)
                if (excludeId.HasValue)
                {
                    query = query.Where(fc => fc.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del carnet en la feria", ex);
            }
        }

        // MÉTODO ORIGINAL MANTENIDO PARA COMPATIBILIDAD (validación global)
        public async Task<bool> ExistsCarnetByIdentificationAsync(int idDocIdentificacion, long noIdentificacion, int? excludeId = null)
        {
            try
            {
                var query = _dbcontext.FeriaCarnets
                    .Where(fc => fc.IdDocIdentificacion == idDocIdentificacion &&
                                fc.NoIdenitficacion == noIdentificacion &&
                                fc.Estatus == true);

                // Si se proporciona excludeId, excluir ese registro (útil para ediciones)
                if (excludeId.HasValue)
                {
                    query = query.Where(fc => fc.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del carnet", ex);
            }
        }

        // Método adicional para obtener carnets por estado de impresión
        public async Task<List<FeriaCarnet>> GetCarnetsByPrintStatusAsync(bool isPrinted)
        {
            try
            {
                return await _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdParticipacionNavigation)
                    .Include(fc => fc.IdDocIdentificacionNavigation)
                    .Include(fc => fc.IdFeriaAreaNavigation)
                        .ThenInclude(fa => fa.IdFeriaNavigation)
                    .Where(fc => fc.Estatus == true && fc.Impresion == isPrinted)
                    .OrderBy(fc => fc.Nombre)
                    .ThenBy(fc => fc.Apellido)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener carnets por estado de impresión", ex);
            }
        }

        // Método adicional para obtener estadísticas básicas
        public async Task<object> GetCarnetStatsAsync()
        {
            try
            {
                var totalCarnets = await _dbcontext.FeriaCarnets.CountAsync(fc => fc.Estatus == true);
                var carnetsPrinted = await _dbcontext.FeriaCarnets.CountAsync(fc => fc.Estatus == true && fc.Impresion == true);
                var carnetsPending = totalCarnets - carnetsPrinted;
                var carnetsWithCourtesy = await _dbcontext.FeriaCarnets.CountAsync(fc => fc.Estatus == true && fc.BoletaCortesia == true);

                return new
                {
                    TotalCarnets = totalCarnets,
                    CarnetsPrinted = carnetsPrinted,
                    CarnetsPending = carnetsPending,
                    CarnetsWithCourtesy = carnetsWithCourtesy
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener estadísticas de carnets", ex);
            }
        }

        // Método adicional para obtener carnets de una feria específica
        public async Task<List<FeriaCarnet>> GetCarnetsByFeriaAsync(int? idFeria)
        {
            try
            {
                if (!idFeria.HasValue)
                    return new List<FeriaCarnet>();

                return await _dbcontext.FeriaCarnets
                    .Include(fc => fc.IdParticipacionNavigation)
                    .Include(fc => fc.IdDocIdentificacionNavigation)
                    .Include(fc => fc.IdFeriaAreaNavigation)
                        .ThenInclude(fa => fa.IdFeriaNavigation)
                    .Where(fc => fc.Estatus == true && fc.IdFeriaAreaNavigation.IdFeria == idFeria.Value)
                    .OrderBy(fc => fc.Nombre)
                    .ThenBy(fc => fc.Apellido)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener carnets de la feria", ex);
            }
        }
    }
}