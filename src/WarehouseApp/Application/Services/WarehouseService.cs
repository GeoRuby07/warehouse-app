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
        public Task<IReadOnlyList<IGrouping<DateTime, Pallet>>> GroupByExpirationAsync() =>
            _palletRepo.ListGroupedByExpirationAsync();

        /// <summary>
        /// Берёт 3 паллеты с наибольшим сроком годности (макс из коробок) и сортирует по возрастанию объёма.
        /// </summary>
        public Task<IReadOnlyList<Pallet>> GetTop3ByMaxBoxExpirationAsync() =>
            _palletRepo.GetTop3ByMaxBoxExpirationAsync();



        public async Task<IEnumerable<Box>> GetAvailableBoxesAsync()
        {
            var all = await _boxRepo.ListAsync();
            return all.Where(b => b.PalletId == null);
        }

        public Task<Box> CreateBoxAsync(Box box)
        {
            // сохранение происходит внутри репозитория
            return _boxRepo.AddAsync(box)
                           .ContinueWith(_ => box);
        }

        public async Task<Pallet> CreatePalletAsync(
            decimal width,
            decimal height,
            decimal depth,
            IEnumerable<Guid> boxIds)
        {
            var boxes = (await _boxRepo.ListAsync())
                .Where(b => boxIds.Contains(b.Id))
                .ToList();

            var pallet = new Pallet(width, height, depth, boxes);
            await _palletRepo.AddAsync(pallet);
            return pallet;
        }
    }
}
