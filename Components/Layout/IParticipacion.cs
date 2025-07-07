using CarnetsdeFeria.Models;

namespace CarnetsdeFeria.Interfaces
{
    public interface IParticipacion
    {
        Task<bool> AddUpdateAsync(Participacion feriaparticipacion);
        Task<bool> DeleteAsync(int id_feriaparticipaion);
        Task<List<Participacion>> GetAllAsync();
        Task<Participacion> GetByIdAsync(int id_feriaparticipacion);

        Task<MPaginatedResult<Participacion>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);
    }
}
