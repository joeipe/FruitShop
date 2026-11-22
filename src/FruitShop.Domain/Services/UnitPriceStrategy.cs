using FruitShop.Domain.Interfaces;

namespace FruitShop.Domain.Services
{
    public sealed class UnitPriceStrategy : IPricingStrategy
    {
        public decimal Calculate(decimal basePrice, decimal quantity)
        {
            return basePrice * quantity;
        }
    }
}