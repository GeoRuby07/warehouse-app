using FluentAssertions;

using Moq;

using WarehouseApp.Application.Repositories;
using WarehouseApp.Application.Services;
using WarehouseApp.Domain;

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
        public async Task GroupByExpiration_ShouldDelegateToRepositoryAsync()
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
            var actual = (await _svc.GroupByExpirationAsync()).ToList();

            // assert
            actual.Should().Equal(expected);
            _palletRepo.Verify(r => r.ListGroupedByExpirationAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTop3ByMaxBoxExpiration_ShouldDelegateToRepositoryAsync()
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
            var actual = (await _svc.GetTop3ByMaxBoxExpirationAsync()).ToList();

            // assert
            actual.Should().Equal(fake);
            _palletRepo.Verify(r => r.GetTop3ByMaxBoxExpirationAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAvailableBoxes_ShouldFilterOutAlreadyAssignedAsync()
        {
            // arrange
            var b1 = new Box { Id = Guid.NewGuid(), PalletId = null };
            var b2 = new Box { Id = Guid.NewGuid(), PalletId = Guid.NewGuid() };
            _boxRepo
                .Setup(r => r.ListAsync())
                .ReturnsAsync([b1, b2]);

            // act
            var actual = (await _svc.GetAvailableBoxesAsync()).ToList();

            // assert
            actual.Should().ContainSingle().Which.Should().Be(b1);
            _boxRepo.Verify(r => r.ListAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBox_ShouldCallRepositoryAndReturnBoxAsync()
        {
            // arrange
            var box = new Box { Id = Guid.NewGuid() };
            _boxRepo
                .Setup(r => r.AddAsync(box))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            var created = await _svc.CreateBoxAsync(box);

            // assert
            created.Should().Be(box);
            _boxRepo.Verify(r => r.AddAsync(box), Times.Once);
        }

        [Fact]
        public async Task CreatePallet_ShouldCallRepositoryWithCorrectBoxesAsync()
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
            var pallet = await _svc.CreatePalletAsync(1, 2, 3, [boxId]);

            // assert
            pallet.Boxes.Should().ContainSingle().Which.Should().Be(box);
            _boxRepo.Verify(r => r.ListAsync(), Times.Once);
            _palletRepo.Verify(r => r.AddAsync(It.Is<Pallet>(p => p.Boxes.Single() == box)), Times.Once);
        }
    }
}
