using CarnetsdeFeria.Models;

namespace CarnetsdeFeria.Interfaces
{
    public interface ICarnet
    {
        Task<bool> AddUpdateAsync(FeriaCarnet carnetFeria);
        Task<bool> DeleteAsync(int id_carnetferia);
        Task<List<FeriaCarnet>> GetAllAsync();
        Task<FeriaCarnet> GetByIdAsync(int id_carnetFeria);

        // Métodos de paginación
        Task<MPaginatedResult<FeriaCarnet>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);
        Task<MPaginatedResult<FeriaCarnet>> GetPaginatedActiveAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);

        // Métodos para obtener datos de dropdowns/selects
        Task<List<FeriaArea>> GetAreaAsync();
        Task<List<Models.Participacion>> GetParticipacionAsync();
        Task<List<DocumentoIdentificacion>> GetIdentificacionAsync();

        // Métodos adicionales para funcionalidades específicas
        Task<bool> ExistsCarnetByIdentificationAsync(int idDocIdentificacion, long noIdentificacion, int? excludeId = null);
        Task<List<FeriaCarnet>> GetCarnetsByPrintStatusAsync(bool isPrinted);
        Task<object> GetCarnetStatsAsync();

    }
}
