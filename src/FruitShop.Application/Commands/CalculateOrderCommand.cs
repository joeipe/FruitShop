using CSharpFunctionalExtensions;
using FruitShop.Application.Interfaces;
using FruitShop.Contracts;
using FruitShop.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FruitShop.Application.Commands
{
    public record CalculateOrderCommand(
        CalculateOrderRequest request,
        IFruitCatalogue catalogue) : IRequest<Result<CalculateOrderResponse?>>
    {
        public Result<CalculateOrderResponse?> Validate()
        {
            var errors = new List<string>();

            foreach (var line in request.Lines)
            {
                var fruit = catalogue.Find(line.FruitCode);

                if (fruit is null)
                {
                    errors.Add($"Fruit code '{line.FruitCode}' not found.");
                    continue;
                }

                if (fruit.Unit == SellingUnit.Item && line.Quantity != decimal.Truncate(line.Quantity))
                {
                    errors.Add($"{fruit.Name} is sold by the item. Enter a whole number.");
                }
            }

            return errors.Count != 0 
                ? Result.Failure<CalculateOrderResponse?>(string.Join(",", errors)) 
                : Result.Success<CalculateOrderResponse?>(null);
        }
    }
}
