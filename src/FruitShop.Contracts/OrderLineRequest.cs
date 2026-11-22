using System.ComponentModel.DataAnnotations;

namespace FruitShop.Contracts
{
    public sealed class OrderLineRequest()
    {
        [Required(ErrorMessage = "Enter a fruit code.")]
        public string FruitCode { get; init; } = string.Empty;

        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            MinimumIsExclusive = true,
            ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; init; }
    }
}