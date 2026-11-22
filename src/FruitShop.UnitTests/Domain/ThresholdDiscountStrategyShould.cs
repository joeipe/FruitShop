using FruitShop.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FruitShop.UnitTests.Domain
{
    public class ThresholdDiscountStrategyShould
    {
        [Fact]
        public void ChargeFullPriceBelowTheThreshold()
        {
            // Arrange
            var sut = new ThresholdDiscountStrategy(
                threshold: 2m, discountRate: 0.10m);

            // Act
            var total = sut.Calculate(basePrice: 5.00m, quantity: 1.5m);

            // Assert
            Assert.Equal(7.50m, total);
        }

        [Fact]
        public void ChargeFullPriceAtTheThreshold()
        {
            // Arrange
            var sut = new ThresholdDiscountStrategy(
                threshold: 2m, discountRate: 0.10m);

            // Act
            var total = sut.Calculate(basePrice: 5.00m, quantity: 2m);

            // Assert
            Assert.Equal(10.00m, total);
        }

        [Fact]
        public void DiscountTheEntireSubtotalAboveTheThreshold()
        {
            // Arrange
            var sut = new ThresholdDiscountStrategy(
                threshold: 2m, discountRate: 0.10m);

            // Act
            var total = sut.Calculate(basePrice: 5.00m, quantity: 3m);

            // Assert
            Assert.Equal(13.50m, total);
        }
    }
}
