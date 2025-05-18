using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;

namespace WarehouseApp.Tests
{
    public class PalletValidationTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenBoxTooWide()
        {
            // box шире паллеты
            var box = new Box { Width = 5, Height = 1, Depth = 1, Weight = 1, ExpirationDateInput = DateTime.Today };
            Action act = () =>
            {
                Pallet pallet = new(width: 4, height: 2, depth: 2, boxes: [box]);
            };
            act.Should().Throw<ArgumentException>()
               .WithMessage("*does not fit into pallet*");
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenBoxTooDeep()
        {
            var box = new Box { Width = 1, Height = 1, Depth = 6, Weight = 1, ExpirationDateInput = DateTime.Today };
            Action act = () =>
            {
                Pallet pallet = new(width: 2, height: 2, depth: 5, boxes: [box]);
            };
            act.Should().Throw<ArgumentException>()
               .WithMessage("*does not fit into pallet*");
        }
    }

    public class PalletPersistenceTests
    {
        private static WarehouseContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<WarehouseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new WarehouseContext(opts);
        }

        [Fact]
        public void Save_Pallet_AssignsPalletIdToBoxes()
        {
            using var ctx = CreateContext();
            // создаём две свободные коробки
            var b1 = new Box { Width = 1, Height = 1, Depth = 1, Weight = 1, ExpirationDateInput = DateTime.Today };
            var b2 = new Box { Width = 1, Height = 1, Depth = 1, Weight = 1, ExpirationDateInput = DateTime.Today };
            ctx.Boxes.AddRange(b1, b2);
            ctx.SaveChanges();

            // сохраняем паллету с этими коробками
            var pallet = new Pallet(2, 2, 2, [b1, b2]);
            ctx.Pallets.Add(pallet);
            ctx.SaveChanges();

            // при загрузке из БД у обеих коробок PalletId == pallet.Id
            var boxes = ctx.Boxes.ToList();
            boxes.All(b => b.PalletId == pallet.Id).Should().BeTrue();
        }
    }
}
