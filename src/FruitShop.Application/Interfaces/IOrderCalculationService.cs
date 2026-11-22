using FruitShop.Domain.Models;

namespace FruitShop.Application.Interfaces
{
    public interface IOrderCalculationService
    {
        decimal CalculateLineTotal(Fruit fruit, decimal quantity);
    }
}