using CarnetsdeFeria.Models;

namespace CarnetsdeFeria.Interfaces
{
    public interface IFeria
    {
        Task<bool> AddUpdateAsync(Ferium feriaevento);
        Task<bool> DeleteAsync(int id_feriaievento);
        Task<List<Ferium>> GetAllAsync();
        Task<Ferium> GetByIdAsync(int id_feriaevento);

        Task<MPaginatedResult<Ferium>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);
    }
}
