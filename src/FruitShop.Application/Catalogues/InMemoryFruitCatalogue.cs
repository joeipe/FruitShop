using FruitShop.Application.Interfaces;
using FruitShop.Domain.Enums;
using FruitShop.Domain.Models;
using FruitShop.Domain.Services;

namespace FruitShop.Application.Catalogues
{
    public class InMemoryFruitCatalogue : IFruitCatalogue
    {
        private readonly Dictionary<string, Fruit> _fruitsByCode;

        public InMemoryFruitCatalogue()
        {
            var unitPrice = new UnitPriceStrategy();

            Fruit[] fruits =
            [
                new("apple", "Apple", SellingUnit.Kilogram, 2.00m, unitPrice),
                new("banana", "Banana", SellingUnit.Item, 0.30m, unitPrice),
                new("cherry", "Cherry", SellingUnit.Kilogram, 5.00m, new ThresholdDiscountStrategy(threshold: 2m, discountRate: 0.10m))
            ];

            _fruitsByCode = fruits.ToDictionary(
                fruit => fruit.Code,
                StringComparer.OrdinalIgnoreCase);
        }

        public Fruit? Find(string code)
        {
            _fruitsByCode.TryGetValue(code, out var fruit);
            return fruit;
        }
    }
}