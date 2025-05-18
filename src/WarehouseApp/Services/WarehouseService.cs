using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Services
{
    public class WarehouseService(WarehouseContext dbContext)
    {
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
            var pallets = _db.Pallets
                                .Include(p => p.Boxes)
                                .AsNoTracking()
                                .ToList();

            return pallets
                .GroupBy(p => p.ExpirationDate)
                .OrderBy(g => g.Key);
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
                        .Max()
                })
                .OrderByDescending(x => x.MaxExpiry)
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
                .ThenBy(p =>
                    p.Boxes.Sum(b => b.Width * b.Height * b.Depth)
                  + (p.Width * p.Height * p.Depth)
                );
        }
    }
}
