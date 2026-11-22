namespace FruitShop.Domain.Interfaces
{
    public interface IPricingStrategy
    {
        decimal Calculate(decimal basePrice, decimal quantity);
    }
}