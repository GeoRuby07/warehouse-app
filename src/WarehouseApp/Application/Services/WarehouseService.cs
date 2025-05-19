using WarehouseApp.Application.Repositories;
using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Application.Services
{
    public class WarehouseService(
        IBoxRepository boxRepository,
        IPalletRepository palletRepository) : IWarehouseService
    {
        private readonly IBoxRepository _boxRepo = boxRepository;
        private readonly IPalletRepository _palletRepo = palletRepository;

        /// <summary>
        /// Группирует паллеты по сроку годности, сортирует группы и паллеты внутри по весу.
        /// </summary>
        public IEnumerable<IGrouping<DateTime, Pallet>> GroupByExpiration() =>
            _palletRepo
                .ListGroupedByExpirationAsync()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        /// Берёт 3 паллеты с наибольшим сроком годности (макс из коробок) и сортирует по возрастанию объёма.
        /// </summary>
        public IEnumerable<Pallet> GetTop3ByMaxBoxExpiration() =>
            _palletRepo
                .GetTop3ByMaxBoxExpirationAsync()
                .GetAwaiter()
                .GetResult();

        public IEnumerable<Box> GetAvailableBoxes() =>
            _boxRepo
                .ListAsync()
                .GetAwaiter()
                .GetResult()
                .Where(b => b.PalletId == null);

        public Box CreateBox(Box box)
        {
            _boxRepo
                .AddAsync(box)
                .GetAwaiter()
                .GetResult();

            return box;
        }

        public Pallet CreatePallet(decimal width, decimal height, decimal depth, IEnumerable<Guid> boxIds)
        {
            var boxes = _boxRepo
                .ListAsync()
                .GetAwaiter()
                .GetResult()
                .Where(b => boxIds.Contains(b.Id))
                .ToList();

            var pallet = new Pallet(width, height, depth, boxes);

            _palletRepo
                .AddAsync(pallet)
                .GetAwaiter()
                .GetResult();

            return pallet;
        }
    }
}
