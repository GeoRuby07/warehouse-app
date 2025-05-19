using FluentAssertions;

using Moq;

using WarehouseApp.Application.Repositories;
using WarehouseApp.Domain;
using WarehouseApp.Services;

namespace WarehouseApp.Tests
{
    public class WarehouseServiceUnitTests
    {
        private readonly Mock<IBoxRepository> _boxRepo = new();
        private readonly Mock<IPalletRepository> _palletRepo = new();
        private readonly WarehouseService _svc;

        public WarehouseServiceUnitTests()
        {
            _svc = new WarehouseService(
                _boxRepo.Object,
                _palletRepo.Object
            );
        }

        [Fact]
        public void GroupByExpiration_ShouldDelegateToRepository()
        {
            // arrange
            var d1 = new DateTime(2025, 1, 1);
            var dummyGroup = new[] { new Pallet(1, 1, 1, []) }
                .GroupBy(p => d1)
                .First();
            var expected = new List<IGrouping<DateTime, Pallet>> { dummyGroup };
            _palletRepo
                .Setup(r => r.ListGroupedByExpirationAsync())
                .ReturnsAsync(expected);

            // act
            var actual = _svc.GroupByExpiration().ToList();

            // assert
            actual.Should().Equal(expected);
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
            var actual = _svc.GetTop3ByMaxBoxExpiration().ToList();

            // assert
            actual.Should().Equal(fake);
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
            var actual = _svc.GetAvailableBoxes().ToList();

            // assert
            actual.Should().ContainSingle().Which.Should().Be(b1);
            _boxRepo.Verify(r => r.ListAsync(), Times.Once);
        }

        [Fact]
        public void CreateBox_ShouldCallRepositoryAndReturnBox()
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
            _boxRepo.Verify(r => r.AddAsync(box), Times.Once);
        }

        [Fact]
        public void CreatePallet_ShouldCallRepositoryWithCorrectBoxes()
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
            _boxRepo.Verify(r => r.ListAsync(), Times.Once);
            _palletRepo.Verify(r => r.AddAsync(It.Is<Pallet>(p => p.Boxes.Single() == box)), Times.Once);
        }
    }
}
