using FruitShop.Domain.Enums;
using FruitShop.Domain.Interfaces;

namespace FruitShop.Domain.Models
{
    public sealed record Fruit(
        string Code,
        string Name,
        SellingUnit Unit,
        decimal BasePrice,
        IPricingStrategy PricingStrategy);
}