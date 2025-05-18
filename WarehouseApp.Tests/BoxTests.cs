using System;

using FluentAssertions;

using WarehouseApp.Domain;

using Xunit;

namespace WarehouseApp.Tests
{
    public class BoxTests
    {
        [Fact]
        public void ExpirationDate_ShouldUseInput_WhenProvided()
        {
            var date = new DateTime(2025, 1, 1);
            var box = new Box
            {
                Width = 1,
                Height = 1,
                Depth = 1,
                Weight = 1,
                ExpirationDateInput = date
            };
            box.ExpirationDate.Should().Be(date);
        }

        [Fact]
        public void ExpirationDate_ShouldComputeFromManufacture_WhenNoInput()
        {
            var mfg = new DateTime(2025, 1, 1);
            var box = new Box
            {
                Width = 1,
                Height = 1,
                Depth = 1,
                Weight = 1,
                ManufactureDate = mfg
            };
            box.ExpirationDate.Should().Be(mfg.AddDays(100));
        }

        [Fact]
        public void Volume_ShouldBeProduct_OfDimensions()
        {
            var box = new Box { Width = 2, Height = 3, Depth = 4, Weight = 1 };
            box.Volume.Should().Be(2 * 3 * 4);
        }
    }
}
