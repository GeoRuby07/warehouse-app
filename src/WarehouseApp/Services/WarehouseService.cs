using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Services
{
    public class WarehouseService(IWarehouseContext dbContext) : IWarehouseService
    {
        private readonly IWarehouseContext _db = dbContext;

        /// <summary>
        /// Доступ к контексту для UI.
        /// </summary>
        public IWarehouseContext GetDbContext() => _db;

        /// <summary>
        /// Группирует паллеты по сроку годности, сортирует группы и паллеты внутри по весу.
        /// </summary>

        public IEnumerable<IGrouping<DateTime, Pallet>> GroupByExpiration()
        {
            var pallets = _db.Pallets
                                .Include(p => p.Boxes)
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

        public IEnumerable<Box> GetAvailableBoxes()
        {
            return [.. _db.Boxes
                      .Where(b => b.PalletId == null)
                      .AsNoTracking()];
        }

        public Box CreateBox(Box box)
        {
            _db.Boxes.Add(box);
            _db.SaveChanges();
            return box;
        }

        public Pallet CreatePallet(decimal width, decimal height, decimal depth, IEnumerable<Guid> boxIds)
        {
            // Загружаем сами Box по id
            var boxes = _db.Boxes
                          .Where(b => boxIds.Contains(b.Id))
                          .ToList();

            var pallet = new Pallet(width, height, depth, boxes);
            _db.Pallets.Add(pallet);

            // EF автоматически установит PalletId у коробок
            _db.SaveChanges();
            return pallet;
        }
    }
}
