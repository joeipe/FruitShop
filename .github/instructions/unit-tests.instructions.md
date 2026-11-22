---
applyTo: "src/FruitShop.UnitTests/**/*.cs"
---

# Unit Test Instructions

- Use xUnit with Arrange-Act-Assert and descriptive test names.
- Name test classes `<ClassName>Should` (e.g. `CalculateOrderCommandHandlerShould`) and methods as behaviors (e.g. `ReturnFailureWhenFruitCodeIsNotFound`). Mirror the source folder structure under `FruitShop.UnitTests`.
- Use Moq to isolate dependencies and verify interactions, such as `CalculateLineTotal` never being called when validation fails. Create mocks per test instance, not in static fields.
- Preserve coverage for these cases:
  - Cherries at exactly 2 kg = 10.00 (no discount) and 3 kg = 13.50 (discount).
  - Mixed order: 1.5 kg apple + 4 banana + 3 kg cherry = 17.70 (3.00 + 1.20 + 13.50).
  - Unknown fruit code returns failure: `Fruit code '<code>' not found.`
  - Fractional item quantity returns failure: `Banana is sold by the item. Enter a whole number.`