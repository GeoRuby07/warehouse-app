using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;

namespace WarehouseApp.Infrastructure
{
    /// <summary>
    /// Абстракция над EF DbContext для работы с коробками и паллетами.
    /// </summary>
    public interface IWarehouseContext
    {
        DbSet<Box> Boxes { get; }
        DbSet<Pallet> Pallets { get; }
        int SaveChanges();
    }
}
