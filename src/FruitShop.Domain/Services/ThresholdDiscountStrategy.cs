using FruitShop.Domain.Interfaces;

namespace FruitShop.Domain.Services
{
    public sealed class ThresholdDiscountStrategy(
        decimal threshold,
        decimal discountRate) : IPricingStrategy
    {
        public decimal Calculate(decimal basePrice, decimal quantity)
        {
            var subtotal = basePrice * quantity;

            if (quantity > threshold)
            {
                return subtotal * (1m - discountRate);
            }

            return subtotal;
        }
    }
}