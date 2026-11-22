# FruitShop Copilot Instructions

## Overview

FruitShop is a .NET 10 ASP.NET Core Web API that calculates fruit order prices using an in-memory catalogue (no database). Nullable reference types and implicit usings are enabled in every project.

Solution: `src/FruitShop.slnx`. Projects live under `src/`.

## Architecture

| Project | Responsibility |
|---|---|
| `FruitShop.API` | HTTP layer: `OrdersController`, `Program.cs` DI composition root, Swagger setup (`Configurations/SwaggerConfig.cs`). |
| `FruitShop.Contracts` | Request/response models: `CalculateOrderRequest`, `OrderLineRequest`, `CalculateOrderResponse`, `PricedOrderLineResponse`. |
| `FruitShop.Application` | `CalculateOrderCommand`, `CalculateOrderCommandHandler`, `IFruitCatalogue`/`InMemoryFruitCatalogue`, `IOrderCalculationService`/`OrderCalculationService`. |
| `FruitShop.Domain` | `Fruit`, `SellingUnit`, `IPricingStrategy`, `UnitPriceStrategy`, `ThresholdDiscountStrategy`. |
| `FruitShop.UnitTests` | xUnit tests (Moq and FluentAssertions available); references `FruitShop.Application`. |

Dependencies flow API -> Application -> Domain. API and Application also use Contracts. Domain and Contracts reference no other project.

### Request flow

`POST api/orders/calculate` (route `api/[controller]/[action]`) ->
`OrdersController.Calculate` builds `CalculateOrderCommand` and calls `IMediator.Send` ->
`CalculateOrderCommandHandler.Handle` runs `command.Validate()` and returns the failed `Result` if validation fails ->
for each line, it looks up the fruit via `IFruitCatalogue.Find` and calls `IOrderCalculationService.CalculateLineTotal`, which delegates to `fruit.PricingStrategy.Calculate`. It maps `SellingUnit` to `"kg"`/`"item"`, then sums the line totals into `CalculateOrderResponse`.

DI (`Program.cs`): MediatR scans the Application assembly. `IFruitCatalogue` is singleton and `IOrderCalculationService` is scoped.

## Pricing Rules

- Use `decimal` for all quantities and prices.
- Catalogue (`InMemoryFruitCatalogue`, codes are case-insensitive):
  - `apple`: kg, 2.00, `UnitPriceStrategy`
  - `banana`: item, 0.30, `UnitPriceStrategy`
  - `cherry`: kg, 5.00, `ThresholdDiscountStrategy(threshold: 2m, discountRate: 0.10m)`
- `UnitPriceStrategy`: `basePrice * quantity`.
- `ThresholdDiscountStrategy`: discounts the **entire line** only when `quantity > threshold` (strictly greater). Exactly 2 kg of cherries costs 10.00; 3 kg costs 13.50.
- Each order line is priced independently. Two 1.5 kg cherry lines do not combine for the discount.
- Fruit sold by `SellingUnit.Item` must have a whole-number quantity.

### Adding a fruit

Add an entry to the `fruits` array in `InMemoryFruitCatalogue` with code, name, `SellingUnit`, base price, and pricing strategy. Reuse `UnitPriceStrategy` for standard pricing. Add tests and update the README table.

### Adding a pricing strategy

Implement `IPricingStrategy.Calculate(decimal basePrice, decimal quantity)` as a sealed class in `FruitShop.Domain/Services`, then assign it to the relevant fruit in the catalogue. Do not add fruit-specific branching to `OrderCalculationService` or the handler.

## API and Validation

Keep the three validation layers separate:

1. **DTO validation** (data annotations on `CalculateOrderRequest`/`OrderLineRequest`, applied by `[ApiController]`): empty order, missing fruit code, quantity <= 0. Returns `400`.
2. **Catalogue and quantity checks** (`CalculateOrderCommand.Validate()`): unknown fruit code, fractional item quantity. Returns `422` with the `Result` error string.
3. **Pricing** (strategies via `OrderCalculationService`): calculation only, no validation.

Preserve the existing request/response contracts and status codes (`200`, `400`, `422`) unless the task requires a change. JSON uses ASP.NET Core default camelCase (`lines`, `fruitCode`, `quantity`, `unit`, `lineTotal`, `total`). Expected failures use `CSharpFunctionalExtensions.Result`, not exceptions.

Swagger UI is available in Development at `/swagger`. The `https` launch profile opens it.

## Commands

Run from the repository root:

```powershell
dotnet build src/FruitShop.slnx
dotnet test src/FruitShop.slnx
dotnet run --project src/FruitShop.API/FruitShop.API.csproj --launch-profile http
```

- `http` profile: `http://localhost:5044`.
- `https` profile: `https://localhost:7186` and `http://localhost:5044`.
- Endpoint: `POST /api/orders/calculate`.

## Coding Style

- Follow existing conventions: block-scoped namespaces, primary constructors for DI, sealed records for contracts and models, PascalCase members.
- Prefer small, focused changes. Reuse existing services and strategies instead of duplicating pricing logic.
- Don't add `using` directives already covered by implicit usings. Remove unused ones in files you edit.
- Update `README.md` when the API contract, validation behavior, or pricing rules change.
- Don't claim a change is verified unless the build and tests were actually run.