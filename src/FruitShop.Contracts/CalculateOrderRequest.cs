using System.ComponentModel.DataAnnotations;

namespace FruitShop.Contracts
{
    public sealed class CalculateOrderRequest()
    {
        [Required(ErrorMessage = "Enter at least one order line.")]
        [MinLength(1, ErrorMessage = "Enter at least one order line.")]
        public List<OrderLineRequest?> Lines { get; init; } = [];
    }
}