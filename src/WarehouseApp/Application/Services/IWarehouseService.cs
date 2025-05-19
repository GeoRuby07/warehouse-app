using WarehouseApp.Domain;

namespace WarehouseApp.Application.Services
{
    /// <summary>
    /// Бизнес-операции над складом
    /// </summary>
    public interface IWarehouseService {
        Task<IReadOnlyList<IGrouping<DateTime, Pallet>>> GroupByExpirationAsync();
        Task<IReadOnlyList<Pallet>> GetTop3ByMaxBoxExpirationAsync();
        Task<IEnumerable<Box>> GetAvailableBoxesAsync();
        Task<Box> CreateBoxAsync(Box box);
        Task<Pallet> CreatePalletAsync(decimal width, decimal height, decimal depth, IEnumerable<Guid> boxIds);
    }
}
