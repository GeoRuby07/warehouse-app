using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using WarehouseApp.Domain;

namespace WarehouseApp.Infrastructure {
    public class WarehouseContext : DbContext {
        private const string DbFileName = "warehouse.db";

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Pallet> Pallets { get; set; }

        public WarehouseContext(DbContextOptions<WarehouseContext> options)
    : base(options)
        {
        }

        public WarehouseContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite($"Data Source={DbFileName}");
            }
        }

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

            // Конвертер decimal <-> double
            var decimalConverter = new ValueConverter<decimal, double>(
                v => (double)v,
                v => (decimal)v);

            foreach (var entity in model.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties()
                                              .Where(p => p.ClrType == typeof(decimal)))
                {
                    property.SetValueConverter(decimalConverter);
                    property.SetColumnType("REAL");
                }
            }
        }
    }
}
