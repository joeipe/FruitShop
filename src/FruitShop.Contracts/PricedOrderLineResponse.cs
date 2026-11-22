namespace FruitShop.Contracts
{
    public sealed record PricedOrderLineResponse(
        string FruitCode,
        decimal Quantity,
        string Unit,
        decimal LineTotal);
}