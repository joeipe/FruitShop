using CSharpFunctionalExtensions;
using FruitShop.Application.Commands;
using FruitShop.Application.Interfaces;
using FruitShop.Contracts;
using FruitShop.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FruitShop.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController(
        IMediator mediator,
        IFruitCatalogue catalogue) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Calculate([FromBody] CalculateOrderRequest request)
        {
            var command = new CalculateOrderCommand(request, catalogue);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value) : StatusCode(StatusCodes.Status422UnprocessableEntity, result.Error);
        }
    }
}