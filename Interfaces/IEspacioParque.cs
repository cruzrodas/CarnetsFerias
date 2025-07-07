using CarnetsdeFeria.Models;

namespace CarnetsdeFeria.Interfaces
{
    public interface IEspacioParque
    {
        Task<bool> AddUpdateAsync(EspacioParque espacioparque);
        Task<bool> DeleteAsync(int id_espacioparque);
        Task<List<EspacioParque>> GetAllAsync();
        Task<EspacioParque> GetByIdAsync(int id_espacioparque);

        Task<MPaginatedResult<EspacioParque>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);
    }
}
