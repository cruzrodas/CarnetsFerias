using CarnetsdeFeria.Models;

namespace CarnetsdeFeria.Interfaces
{
    public interface IDocIdentificacion
    {
        Task<bool> AddUpdateAsync(DocumentoIdentificacion feriaidentificacion);
        Task<bool> DeleteAsync(int id_feriaidentificacion);
        Task<List<DocumentoIdentificacion>> GetAllAsync();
        Task<DocumentoIdentificacion> GetByIdAsync(int id_feriaIdentificacion);

        Task<MPaginatedResult<DocumentoIdentificacion>> GetPaginatedAsync(int pageNumber, int pageSize, string searchTerm = "", bool sortAscending = true);
    }
}
