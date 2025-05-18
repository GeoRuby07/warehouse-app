using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Services {
    public class WarehouseService(WarehouseContext dbContext) {
        private readonly WarehouseContext _db = dbContext;

        /// <summary>
        /// Группирует паллеты по сроку годности, сортирует группы и паллеты внутри по весу.
        /// </summary>
        public IEnumerable<IGrouping<DateTime, Pallet>> GroupByExpiration()
        {
            // Группируем по сроку и сортируем группы по ключу (дате):
            return _db.Pallets
                      .ToList()
                      .GroupBy(p => p.ExpirationDate)
                      .OrderBy(g => g.Key);
        }

        /// <summary>
        /// Берёт 3 паллеты с наибольшим сроком годности (макс из коробок) и сортирует по возрастанию объёма.
        /// </summary>
        public IEnumerable<Pallet> GetTop3ByMaxBoxExpiration()
        {
            return [.. _db.Pallets
                      .ToList()
                      .OrderByDescending(p => p.Boxes.Max(b => b.ExpirationDate))
                      .Take(3)
                      .OrderBy(p => p.Volume)];
        }
    }
}
