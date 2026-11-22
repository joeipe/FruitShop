using FruitShop.Application.Services;
using FruitShop.Domain.Enums;
using FruitShop.Domain.Interfaces;
using FruitShop.Domain.Models;
using FruitShop.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FruitShop.UnitTests.Services
{
    public class OrderCalculationServiceShould
    {
        [Fact]
        public void CalculateLineTotalUsingTheAssignedPricingStrategy()
        {
            // Arrange
            var mockStrategy = new Mock<IPricingStrategy>();
            mockStrategy
                .Setup(x => x.Calculate(5.00m, 3m))
                .Returns(13.50m);

            var cherry = new Fruit(
                "cherry", "Cherry", SellingUnit.Kilogram, 5.00m,
                mockStrategy.Object);

            var sut = new OrderCalculationService();

            // Act
            var total = sut.CalculateLineTotal(cherry, 3m);

            // Assert
            Assert.Equal(13.50m, total);
            mockStrategy.Verify(x => x.Calculate(5.00m, 3m), Times.Once);
        }
    }
}