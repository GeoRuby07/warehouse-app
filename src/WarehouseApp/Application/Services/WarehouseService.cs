using Microsoft.Extensions.Logging;

using WarehouseApp.Application.Repositories;
using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Application.Services
{
    public class WarehouseService(
        IBoxRepository boxRepository,
        IPalletRepository palletRepository,
        ILogger<WarehouseService> logger) : IWarehouseService
    {
        private readonly IBoxRepository _boxRepo = boxRepository;
        private readonly IPalletRepository _palletRepo = palletRepository;
        private readonly ILogger<WarehouseService> _logger = logger;

        /// <summary>
        /// Группирует паллеты по сроку годности, сортирует группы и паллеты внутри по весу.
        /// </summary>
        public async Task<IReadOnlyList<IGrouping<DateTime, Pallet>>> GroupByExpirationAsync()
        {
            _logger.LogInformation("Grouping pallets by expiration…");
            var groups = await _palletRepo.ListGroupedByExpirationAsync();
            _logger.LogInformation("Found {Count} expiration groups", groups.Count);
            return groups;
        }

        /// <summary>
        /// Берёт 3 паллеты с наибольшим сроком годности (макс из коробок) и сортирует по возрастанию объёма.
        /// </summary>
        public async Task<IReadOnlyList<Pallet>> GetTop3ByMaxBoxExpirationAsync()
        {
            _logger.LogInformation("Fetching top3 pallets by max box expiration…");
            var result = await _palletRepo.GetTop3ByMaxBoxExpirationAsync();
            _logger.LogInformation("Top3 fetched: {Ids}", string.Join(',', result.Select(p => p.Id)));
            return result;
        }



        public async Task<IEnumerable<Box>> GetAvailableBoxesAsync()
        {
            _logger.LogDebug("Listing available boxes…");
            var all = await _boxRepo.ListAsync();
            var free = all.Where(b => b.PalletId == null).ToList();
            _logger.LogDebug("{Count} boxes available", free.Count);
            return free;
        }

        public async Task<Box> CreateBoxAsync(Box box)
        {
            _logger.LogInformation("Creating new box {Box}", box);
            await _boxRepo.AddAsync(box);
            _logger.LogInformation("Box added to repository, id={Id}", box.Id);
            return box;
        }

        public async Task<Pallet> CreatePalletAsync(decimal w, decimal h, decimal d, IEnumerable<Guid> boxIds)
        {
            _logger.LogInformation("Creating new pallet {W}x{H}x{D} with boxes: {Ids}", w, h, d, boxIds);
            var boxes = (await _boxRepo.ListAsync()).Where(b => boxIds.Contains(b.Id)).ToList();
            var pallet = new Pallet(w, h, d, boxes);
            await _palletRepo.AddAsync(pallet);
            _logger.LogInformation("Pallet created id={Id} contains {Count} boxes", pallet.Id, boxes.Count);
            return pallet;
        }
    }
}
