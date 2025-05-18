using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;
using WarehouseApp.Services;

namespace WarehouseApp.Tests
{
    public class WarehouseServiceTests
    {
        private static WarehouseContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<WarehouseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new WarehouseContext(opts);
        }

        [Fact]
        public void GroupByExpiration_ShouldGroupAndSort()
        {
            using var ctx = CreateContext();
            var svc = new WarehouseService(ctx);
            var d1 = new DateTime(2025, 1, 1);
            var d2 = new DateTime(2025, 1, 2);

            var p1 = new Pallet(1, 1, 1, [new Box { Width = 1, Height = 1, Depth = 1, Weight = 1, ExpirationDateInput = d2 }]);
            var p2 = new Pallet(1, 1, 1, [new Box { Width = 1, Height = 1, Depth = 1, Weight = 1, ExpirationDateInput = d1 }]);

            ctx.Pallets.AddRange(p1, p2);
            ctx.SaveChanges();

            var groups = svc.GroupByExpiration().ToList();
            groups.Should().HaveCount(2);
            groups[0].Key.Should().Be(d1);
            groups[1].Key.Should().Be(d2);
            groups[0].OrderBy(p => p.Weight).First().Should().Be(p2);
        }

        [Fact]
        public void GetTop3ByMaxBoxExpiration_ShouldReturnCorrectOrder()
        {
            using var ctx = CreateContext();
            var svc = new WarehouseService(ctx);

            // Exp: 1,2,3,4
            var pallets = Enumerable.Range(1, 4)
                .Select(i => new Pallet(1, 1, 1,
                    [new Box { Width = 1, Height = 1, Depth = 1, Weight = 1, ExpirationDateInput = new DateTime(2025, 1, i) }]))
                .ToList();
            ctx.Pallets.AddRange(pallets);
            ctx.SaveChanges();

            var top3 = svc.GetTop3ByMaxBoxExpiration().ToList();
            // Should pick expiry 4,3,2 then order by volume (volume equal)
            top3.Should().HaveCount(3);
            top3.Select(p => p.Boxes.Max(b => b.ExpirationDate)).Should()
                .ContainInOrder([new DateTime(2025, 1, 4), new DateTime(2025, 1, 3), new DateTime(2025, 1, 2)]);
        }
    }
}
