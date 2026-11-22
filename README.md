# Fruit Shop Order Calculator

A .NET 10 ASP.NET Core Web API that prices fruit sold by weight or by the item. It returns the price of each order line and the total for the order.

| Fruit | Unit price | Pricing rule |
|---|---:|---|
| Apple | $2.00 per kg | Standard unit price |
| Banana | $0.30 per item | Standard unit price. The quantity must be a whole number. |
| Cherry | $5.00 per kg | 10% off the full cherry line when the quantity exceeds 2 kg |

## Run the API

Install the .NET 10 SDK. From the solution root, run:

```bash
dotnet run --project FruitShop.API/FruitShop.API.csproj --no-launch-profile -- --urls http://localhost:7186
```

The fruit catalogue is stored in memory, so the API can run without database setup.

## Calculate an order

With the API running, send this request in a second terminal:

```bash
curl -sS http://localhost:7186/api/orders/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "lines": [
      { "fruitCode": "apple", "quantity": 1.5 },
      { "fruitCode": "banana", "quantity": 4 },
      { "fruitCode": "cherry", "quantity": 3 }
    ]
  }'
```

The command prints the JSON response, including the **$17.70** order total.

```json
{
  "lines": [
    { "fruitCode": "apple", "quantity": 1.5, "unit": "kg", "lineTotal": 3.00 },
    { "fruitCode": "banana", "quantity": 4, "unit": "item", "lineTotal": 1.20 },
    { "fruitCode": "cherry", "quantity": 3, "unit": "kg", "lineTotal": 13.50 }
  ],
  "total": 17.70
}
```

Request model validation returns `400 Bad Request` for invalid input such as an empty order. The command returns a validation failure for an unknown fruit code or a fractional item count, which the controller maps to `422`.

Each submitted line is priced independently. A single cherry line of 3 kg qualifies for the discount. Two separate cherry lines of 1.5 kg each are priced at $7.50 each.

## Design

| Project | Responsibility |
|---|---|
| `FruitShop.API` | Receives the HTTP request, sends `CalculateOrderCommand` through MediatR, and maps the result to an HTTP response. |
| `FruitShop.Contracts` | Defines the request and response models. |
| `FruitShop.Application` | Provides the in-memory catalogue, validates the order, prices each line, and sums the line totals. |
| `FruitShop.Domain` | Defines `Fruit`, `SellingUnit`, and the pricing strategies. |

Quantities and prices use `decimal`. A quantity can represent a fractional weight or a whole item count. The catalogue supplies the selling unit and base price for each fruit.

### Strategy pattern

Each `Fruit` has an `IPricingStrategy`. `OrderCalculationService` calls the strategy assigned to that fruit to calculate its line total.

`UnitPriceStrategy` multiplies the base price by the quantity and is shared by apples and bananas. `ThresholdDiscountStrategy` applies a configurable percentage discount when the quantity exceeds a configured threshold. This lets the order calculation use the same method for every fruit while each strategy implements its own formula.

### Extending the calculator

- **Add a fruit.** Add a catalogue entry with a code, name, selling unit, base price, and pricing strategy. A pear sold at $3.00 per kg can reuse `UnitPriceStrategy`.
- **Change a discount.** Update the threshold and discount rate when creating `ThresholdDiscountStrategy` in the catalogue.
- **Add a pricing rule.** Implement `IPricingStrategy` and assign the new implementation to the relevant fruit. A buy-three-pay-for-two rule would use the same `Calculate` method as the existing strategies.

## Tests

Run the xUnit project from the solution root with `dotnet test` once it has been added to the solution. Use Arrange–Act–Assert to cover:

- Standard pricing for 1.5 kg of apples and four bananas.
- Cherries at exactly 2 kg ($10.00) and above the threshold at 3 kg ($13.50).
- Validation failures for an unknown fruit code and a fractional banana quantity.
- A mixed order with an expected total of $17.70.