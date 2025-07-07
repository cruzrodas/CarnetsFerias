using CarnetsdeFeria.Models;

namespace CarnetsdeFeria.Interfaces
{
    public interface IAreasdeFeria
    {
        Task<bool> AddUpdateAsync(FeriaArea areasdeferia);
        Task<bool> DeleteAsync(int id_areasdeferia);
        Task<List<FeriaArea>> GetAllAsync();
        Task<FeriaArea> GetByIdAsync(int id_areasdeferia);

        Task<MPaginatedResult<FeriaArea>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);

        Task<MPaginatedResult<FeriaArea>> GetPaginatedActiveAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);

        Task<List<EspacioParque>> GetEspaciosAsync();
        Task<List<Ferium>> GetFeriaAsync();
    }
}
