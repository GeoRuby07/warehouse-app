using Microsoft.EntityFrameworkCore;

using WarehouseApp.Application.Repositories;
using WarehouseApp.Domain;

namespace WarehouseApp.Infrastructure.Repositories
{
    public class PalletRepository(WarehouseContext ctx) : IPalletRepository
    {
        private readonly WarehouseContext _ctx = ctx;

        public async Task<Pallet?> GetByIdAsync(Guid id) =>
            await _ctx.Pallets.FindAsync(id);

        public async Task<IReadOnlyList<Pallet>> ListAsync() =>
            await _ctx.Pallets
                      .Include(p => p.Boxes)
                      .ToListAsync();

        public async Task AddAsync(Pallet pallet)
        {
            await _ctx.Pallets.AddAsync(pallet);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<IGrouping<DateTime, Pallet>>> ListGroupedByExpirationAsync()
        {
            var all = await _ctx.Pallets
                                .Include(p => p.Boxes)
                                .ToListAsync();

            return [.. all
                .Where(p => p.Boxes.Any())
                .GroupBy(p => p.ExpirationDate)
                .OrderBy(g => g.Key)];
        }

        public async Task<IReadOnlyList<Pallet>> GetTop3ByMaxBoxExpirationAsync()
        {
            var list = await _ctx.Pallets
                .Include(p => p.Boxes)
                .Where(p => p.Boxes.Any())
                .ToListAsync();

            return [.. list
                .OrderByDescending(p => p.Boxes.Max(b => b.ExpirationDate))
                .Take(3)
                .OrderBy(p => p.Boxes.Sum(b => b.Volume) + p.Volume - p.Width * p.Height * p.Depth)];
        }
    }
}
