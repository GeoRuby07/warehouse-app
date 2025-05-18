using WarehouseApp.Domain;

namespace WarehouseApp.Application.Repositories
{
    public interface IBoxRepository
    {
        Task<Box?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Box>> ListAsync();
        Task AddAsync(Box box);
    }
}
