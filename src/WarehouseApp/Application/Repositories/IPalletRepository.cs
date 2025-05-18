using WarehouseApp.Domain;

namespace WarehouseApp.Application.Repositories
{
    public interface IPalletRepository
    {
        Task<Pallet?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Pallet>> ListAsync();
        Task AddAsync(Pallet pallet);
        // сюда вынесем методы группировки и выборки:
        Task<IReadOnlyList<IGrouping<DateTime, Pallet>>> ListGroupedByExpirationAsync();
        Task<IReadOnlyList<Pallet>> GetTop3ByMaxBoxExpirationAsync();
    }
}
