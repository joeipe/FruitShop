using FruitShop.Application.Interfaces;
using FruitShop.Domain.Enums;
using FruitShop.Domain.Models;

namespace FruitShop.Application.Services
{
    public class OrderCalculationService : IOrderCalculationService
    {
        public decimal CalculateLineTotal(Fruit fruit, decimal quantity)
        {
            return fruit.PricingStrategy.Calculate(fruit.BasePrice, quantity);
        }
    }
}