using WarehouseApp.Domain;

namespace WarehouseApp.Application.Services
{
    /// <summary>
    /// Бизнес-операции над складом
    /// </summary>
    public interface IWarehouseService
    {
        IEnumerable<IGrouping<DateTime, Pallet>> GroupByExpiration();
        IEnumerable<Pallet> GetTop3ByMaxBoxExpiration();

        IEnumerable<Box> GetAvailableBoxes();
        Box CreateBox(Box box);
        Pallet CreatePallet(decimal width, decimal height, decimal depth, IEnumerable<Guid> boxIds);
    }
}
