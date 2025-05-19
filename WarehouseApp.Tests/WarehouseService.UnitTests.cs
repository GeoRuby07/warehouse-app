using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;
using WarehouseApp.Services;
using WarehouseApp.Application.Repositories; // IPalletRepository, IBoxRepository
using Xunit;

namespace WarehouseApp.Tests
{
    public class WarehouseServiceUnitTests
    {
        private readonly Mock<IWarehouseContext> _dbMock = new();
        private readonly Mock<IBoxRepository> _boxRepo = new();
        private readonly Mock<IPalletRepository> _palletRepo = new();
        private readonly WarehouseService _svc;

        public WarehouseServiceUnitTests()
        {
            _svc = new WarehouseService(
                _dbMock.Object,
                _boxRepo.Object,
                _palletRepo.Object
            );
        }

        [Fact]
        public void GroupByExpiration_ShouldDelegateToRepository()
        {
            // arrange
            var d1 = new DateTime(2025, 1, 1);
            var grouping = new[] { new Pallet(1, 1, 1, []) }
                .GroupBy(p => d1)
                .First();
            var dummy = new List<IGrouping<DateTime, Pallet>> { grouping };

            _palletRepo
                .Setup(r => r.ListGroupedByExpirationAsync())
                .ReturnsAsync(dummy);

            // act
            var result = _svc.GroupByExpiration().ToList();

            // assert
            result.Should().Equal(dummy);
            _palletRepo.Verify(r => r.ListGroupedByExpirationAsync(), Times.Once);
        }

        [Fact]
        public void GetTop3ByMaxBoxExpiration_ShouldDelegateToRepository()
        {
            // arrange
            var fake = new List<Pallet>
            {
                new(1,1,1, []),
                new(2,2,2, [])
            };
            _palletRepo
                .Setup(r => r.GetTop3ByMaxBoxExpirationAsync())
                .ReturnsAsync(fake);

            // act
            var result = _svc.GetTop3ByMaxBoxExpiration().ToList();

            // assert
            result.Should().Equal(fake);
            _palletRepo.Verify(r => r.GetTop3ByMaxBoxExpirationAsync(), Times.Once);
        }

        [Fact]
        public void GetAvailableBoxes_ShouldFilterOutAlreadyAssigned()
        {
            // arrange
            var b1 = new Box { Id = Guid.NewGuid(), PalletId = null };
            var b2 = new Box { Id = Guid.NewGuid(), PalletId = Guid.NewGuid() };
            _boxRepo
                .Setup(r => r.ListAsync())
                .ReturnsAsync([b1, b2]);

            // act
            var result = _svc.GetAvailableBoxes().ToList();

            // assert
            result.Should().ContainSingle().Which.Should().Be(b1);
            _boxRepo.Verify(r => r.ListAsync(), Times.Once);
        }

        [Fact]
        public void CreateBox_ShouldAddAndSave()
        {
            // arrange
            var box = new Box { Id = Guid.NewGuid() };
            _boxRepo
                .Setup(r => r.AddAsync(box))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            var created = _svc.CreateBox(box);

            // assert
            created.Should().Be(box);
            _boxRepo.Verify();
            _dbMock.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CreatePallet_ShouldAddAndSave()
        {
            // arrange
            var boxId = Guid.NewGuid();
            var box = new Box { Id = boxId };
            _boxRepo
                .Setup(r => r.ListAsync())
                .ReturnsAsync([box]);
            _palletRepo
                .Setup(r => r.AddAsync(It.IsAny<Pallet>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            var pallet = _svc.CreatePallet(1, 2, 3, [boxId]);

            // assert
            pallet.Boxes.Should().ContainSingle().Which.Should().Be(box);
            _palletRepo.Verify();
            _dbMock.Verify(db => db.SaveChanges(), Times.Once);
        }
    }
}
