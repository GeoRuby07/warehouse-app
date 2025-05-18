using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;

namespace WarehouseApp.Infrastructure {
    public class WarehouseContext : DbContext {
        private const string DbFileName = "warehouse.db";

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Pallet> Pallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbFileName}");

        protected override void OnModelCreating(ModelBuilder model)
        {
            // Даты храним как DATE
            model.Entity<Box>()
                 .Property(b => b.ManufactureDate)
                 .HasColumnType("DATE");

            model.Entity<Box>()
                 .Property(b => b.ExpirationDateInput)
                 .HasColumnType("DATE");

            // ExpirationDate/Volume вычисляемые
            model.Entity<Pallet>()
                 .Ignore(p => p.ExpirationDate)
                 .Ignore(p => p.Volume);
        }
    }
}
