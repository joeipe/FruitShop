using FruitShop.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FruitShop.UnitTests.Domain
{
    public class UnitPriceStrategyShould
    {
        [Fact]
        public void CalculatePriceForFractionalWeight()
        {
            // Arrange
            var sut = new UnitPriceStrategy();

            // Act
            var total = sut.Calculate(basePrice: 2.00m, quantity: 1.5m);

            // Assert
            Assert.Equal(3.00m, total);
        }

        [Fact]
        public void CalculatePriceForWholeItemCount()
        {
            // Arrange
            var sut = new UnitPriceStrategy();

            // Act
            var total = sut.Calculate(basePrice: 0.30m, quantity: 4m);

            // Assert
            Assert.Equal(1.20m, total);
        }
    }
}
