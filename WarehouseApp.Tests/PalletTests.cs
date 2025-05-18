using System;

using FluentAssertions;

using WarehouseApp.Domain;

using Xunit;

namespace WarehouseApp.Tests
{
    public class PalletTests
    {
        private static Box MakeBox(decimal w, decimal d, DateTime exp, decimal wt = 1)
            => new()
            {
                Width = w,
                Height = 1,
                Depth = d,
                Weight = wt,
                ExpirationDateInput = exp
            };

        [Fact]
        public void ExpirationDate_IsMinOfBoxExpiries()
        {
            var b1 = MakeBox(1, 1, new DateTime(2025, 1, 10));
            var b2 = MakeBox(1, 1, new DateTime(2025, 1, 5));
            var pallet = new Pallet(2, 2, 2, [b1, b2]);
            pallet.ExpirationDate.Should().Be(b2.ExpirationDate);
        }

        [Fact]
        public void Weight_IsSumOfBoxesPlus30()
        {
            var b1 = MakeBox(1, 1, DateTime.Today, 2);
            var b2 = MakeBox(1, 1, DateTime.Today, 3);
            var pallet = new Pallet(2, 2, 2, [b1, b2]);
            pallet.Weight.Should().Be(2 + 3 + 30);
        }

        [Fact]
        public void Volume_IsOwnPlusBoxes()
        {
            var b1 = MakeBox(1, 1, DateTime.Today);
            var b2 = MakeBox(1, 1, DateTime.Today);
            var pallet = new Pallet(2, 2, 2, [b1, b2]);
            var expected = (2m * 2 * 2) + b1.Volume + b2.Volume;
            pallet.Volume.Should().Be(expected);
        }
    }
}
