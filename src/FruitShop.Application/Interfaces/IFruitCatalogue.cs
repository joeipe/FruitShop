using FruitShop.Domain.Models;

namespace FruitShop.Application.Interfaces
{
    public interface IFruitCatalogue
    {
        Fruit? Find(string code);
    }
}