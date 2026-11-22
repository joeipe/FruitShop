using FruitShop.Application.CommandHandlers;
using FruitShop.Application.Commands;
using FruitShop.Application.Interfaces;
using FruitShop.Contracts;
using FruitShop.Domain.Enums;
using FruitShop.Domain.Models;
using FruitShop.Domain.Services;
using Moq;

namespace FruitShop.UnitTests.CommandHandlers
{
    public class CalculateOrderCommandHandlerShould
    {
        private readonly Mock<IFruitCatalogue> _mockCatalogue;
        private readonly Mock<IOrderCalculationService> _mockOrderCalculationService;

        public CalculateOrderCommandHandlerShould()
        {
            _mockCatalogue = new Mock<IFruitCatalogue>();
            _mockOrderCalculationService = new Mock<IOrderCalculationService>();
        }

        [Fact]
        public async Task CalculateTotalForFourBananas()
        {
            // Arrange
            var banana = new Fruit(
                "banana", "Banana", SellingUnit.Item, 0.30m,
                new UnitPriceStrategy());

            var request = new CalculateOrderRequest
            {
                Lines =
                [
                    new OrderLineRequest { FruitCode = "banana", Quantity = 4m }
                ]
            };

            _mockCatalogue
                .Setup(x => x.Find("banana"))
                .Returns(banana);

            _mockOrderCalculationService
                .Setup(x => x.CalculateLineTotal(banana, 4m))
                .Returns(1.20m);

            var command = new CalculateOrderCommand(request, _mockCatalogue.Object);
            var sut = new CalculateOrderCommandHandler(
                _mockCatalogue.Object, _mockOrderCalculationService.Object);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            var response = Assert.IsType<CalculateOrderResponse>(result.Value);
            var line = Assert.Single(response.Lines);

            Assert.Equal("banana", line.FruitCode);
            Assert.Equal(4m, line.Quantity);
            Assert.Equal("item", line.Unit);
            Assert.Equal(1.20m, line.LineTotal);
            Assert.Equal(1.20m, response.Total);

            _mockOrderCalculationService.Verify(
                x => x.CalculateLineTotal(banana, 4m), Times.Once);
        }

        [Fact]
        public async Task CalculateTotalForMultipleFruitLines()
        {
            var apple = new Fruit(
                "apple", "Apple", SellingUnit.Kilogram, 2.00m,
                new UnitPriceStrategy());
            var banana = new Fruit(
                "banana", "Banana", SellingUnit.Item, 0.30m,
                new UnitPriceStrategy());
            var request = new CalculateOrderRequest
            {
                Lines =
                [
                    new OrderLineRequest { FruitCode = "apple", Quantity = 1.5m },
                    new OrderLineRequest { FruitCode = "banana", Quantity = 4m }
                ]
            };

            _mockCatalogue.Setup(x => x.Find("apple")).Returns(apple);
            _mockCatalogue.Setup(x => x.Find("banana")).Returns(banana);
            _mockOrderCalculationService
                .Setup(x => x.CalculateLineTotal(apple, 1.5m))
                .Returns(3.00m);
            _mockOrderCalculationService
                .Setup(x => x.CalculateLineTotal(banana, 4m))
                .Returns(1.20m);

            var command = new CalculateOrderCommand(request, _mockCatalogue.Object);
            var sut = new CalculateOrderCommandHandler(
                _mockCatalogue.Object, _mockOrderCalculationService.Object);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            var response = Assert.IsType<CalculateOrderResponse>(result.Value);
            Assert.Equal(2, response.Lines.Count);
            Assert.Equal("kg", response.Lines[0].Unit);
            Assert.Equal(3.00m, response.Lines[0].LineTotal);
            Assert.Equal("item", response.Lines[1].Unit);
            Assert.Equal(1.20m, response.Lines[1].LineTotal);
            Assert.Equal(4.20m, response.Total);
        }

        [Fact]
        public async Task ReturnFailureWhenFruitCodeIsNotFound()
        {
            var request = new CalculateOrderRequest
            {
                Lines = [new OrderLineRequest { FruitCode = "unknown", Quantity = 1m }]
            };
            _mockCatalogue.Setup(x => x.Find("unknown")).Returns((Fruit?)null);

            var command = new CalculateOrderCommand(request, _mockCatalogue.Object);
            var sut = new CalculateOrderCommandHandler(
                _mockCatalogue.Object, _mockOrderCalculationService.Object);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal("Fruit code 'unknown' not found.", result.Error);
            _mockOrderCalculationService.Verify(
                x => x.CalculateLineTotal(It.IsAny<Fruit>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Fact]
        public async Task ReturnFailureWhenItemQuantityIsFractional()
        {
            var banana = new Fruit(
                "banana", "Banana", SellingUnit.Item, 0.30m,
                new UnitPriceStrategy());
            var request = new CalculateOrderRequest
            {
                Lines = [new OrderLineRequest { FruitCode = "banana", Quantity = 1.5m }]
            };
            _mockCatalogue.Setup(x => x.Find("banana")).Returns(banana);

            var command = new CalculateOrderCommand(request, _mockCatalogue.Object);
            var sut = new CalculateOrderCommandHandler(
                _mockCatalogue.Object, _mockOrderCalculationService.Object);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal("Banana is sold by the item. Enter a whole number.", result.Error);
            _mockOrderCalculationService.Verify(
                x => x.CalculateLineTotal(It.IsAny<Fruit>(), It.IsAny<decimal>()),
                Times.Never);
        }
    }
}
