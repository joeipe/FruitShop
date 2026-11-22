using CSharpFunctionalExtensions;
using FruitShop.Application.Commands;
using FruitShop.Application.Interfaces;
using FruitShop.Contracts;
using FruitShop.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FruitShop.Application.CommandHandlers
{
    public class CalculateOrderCommandHandler(
        IFruitCatalogue catalogue,
        IOrderCalculationService orderCalculationService) : IRequestHandler<CalculateOrderCommand, Result<CalculateOrderResponse?>>
    {
        public Task<Result<CalculateOrderResponse?>> Handle(CalculateOrderCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();

            if (validation.IsFailure)
            {
                return Task.FromResult(validation);
            }

            var pricedLines = new List<PricedOrderLineResponse>();

            foreach (var line in request.request.Lines)
            {
                var fruit = catalogue.Find(line!.FruitCode);

                decimal lineTotal = 0;
                lineTotal = orderCalculationService.CalculateLineTotal(fruit!, line.Quantity);

                var unit = fruit!.Unit == SellingUnit.Kilogram ? "kg" : "item";

                pricedLines.Add(new PricedOrderLineResponse(fruit.Code, line.Quantity, unit, lineTotal));
            }

            var total = pricedLines.Sum(line => line.LineTotal);
            var response = new CalculateOrderResponse(pricedLines, total);

            return Task.FromResult(Result.Success<CalculateOrderResponse?>(response));
        }
    }
}
