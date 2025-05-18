using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Services {
    public class WarehouseService(WarehouseContext dbContext) {
        private readonly WarehouseContext _db = dbContext;

        /// <summary>
        /// Доступ к контексту для UI.
        /// </summary>
        public WarehouseContext GetDbContext() => _db;

        /// <summary>
        /// Группирует паллеты по сроку годности, сортирует группы и паллеты внутри по весу.
        /// </summary>

        public IEnumerable<IGrouping<DateTime, Pallet>> GroupByExpiration()
        {
            var stats = _db.Pallets
                .Where(p => p.Boxes.Any())
                .Select(p => new
                {
                    Pallet = p,
                    ExpirationMin = p.Boxes
                        .Select(b => b.ExpirationDateInput ?? b.ManufactureDate!.Value.AddDays(100))
                        .Min(),
                    WeightCalc = p.Boxes.Sum(b => b.Weight) + 30m
                })
                .OrderBy(x => x.ExpirationMin)
                .ThenBy(x => x.WeightCalc)
                .AsEnumerable();

            return stats
                .GroupBy(x => x.ExpirationMin, x => x.Pallet);
        }

        /// <summary>
        /// Берёт 3 паллеты с наибольшим сроком годности (макс из коробок) и сортирует по возрастанию объёма.
        /// </summary>

        public IEnumerable<Pallet> GetTop3ByMaxBoxExpiration()
        {
            var topIds = _db.Pallets
                .Where(p => p.Boxes.Any())
                .Select(p => new
                {
                    p.Id,
                    MaxExpiry = p.Boxes
                        .Select(b => b.ExpirationDateInput ?? b.ManufactureDate!.Value.AddDays(100))
                        .Max(),
                    VolumeCalc = Convert.ToDouble(
                        p.Boxes.Sum(b => b.Width * b.Height * b.Depth)
                      + (p.Width * p.Height * p.Depth))
                })
                .OrderByDescending(x => x.MaxExpiry)
                .ThenBy(x => x.VolumeCalc)
                .Take(3)
                .Select(x => x.Id)
                .ToList();

            var pallets = _db.Pallets
                .Where(p => topIds.Contains(p.Id))
                .Include(p => p.Boxes)
                .AsNoTracking()
                .ToList();

            return pallets
                .OrderByDescending(p => p.Boxes
                    .Select(b => b.ExpirationDateInput ?? b.ManufactureDate!.Value.AddDays(100))
                    .Max())
                .ThenBy(p => p.Volume);
        }
    }
}
