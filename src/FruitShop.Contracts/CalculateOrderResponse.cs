namespace FruitShop.Contracts
{
    public sealed record CalculateOrderResponse(
        IReadOnlyList<PricedOrderLineResponse> Lines,
        decimal Total);
}