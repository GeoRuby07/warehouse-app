using Microsoft.EntityFrameworkCore;

using WarehouseApp.Application.Repositories;
using WarehouseApp.Domain;

namespace WarehouseApp.Infrastructure.Repositories
{
    public class BoxRepository(WarehouseContext ctx) : IBoxRepository
    {
        private readonly WarehouseContext _ctx = ctx;

        public async Task<Box?> GetByIdAsync(Guid id) =>
            await _ctx.Boxes.FindAsync(id);

        public async Task<IReadOnlyList<Box>> ListAsync() =>
            await _ctx.Boxes.ToListAsync();

        public async Task AddAsync(Box box)
        {
            await _ctx.Boxes.AddAsync(box);
            await _ctx.SaveChangesAsync();
        }
    }
}
