using System.ComponentModel.DataAnnotations;

using Server.Attributes.Shared;
using Server.Entities;
using Server.DTO.Products;
using System.ComponentModel.DataAnnotations.Schema;
using Server.Attributes.Products;
using Swashbuckle.AspNetCore.Annotations;

namespace Server.DTO.Products
{
    public class CreateProductDTO
    {
        [Required]
        [SwaggerParameter("Product code.")]
        public string Code { get; set; } = null!;

        [Required]
        [SwaggerParameter("Product name.")]
        [UniqueProductNameValidator]
        public string ProductName { get; set; } = null!;

        [Required]
        [SwaggerParameter("Product description.")]
        [MaxLength(300)]
        public string Description { get; set; } = null!;

        [Required]
        [SwaggerParameter("Product unit price.")]
        [ProductUnitPriceValidator]
        public decimal UnitPrice { get; set; }

        [Required]
        [SwaggerParameter("Product units in stock.")]
        [ProductUnitsInStockValidator]
        public short UnitsInStock { get; set; }

        [Required]
        [SwaggerParameter("Product units on order.")]
        [ProductUnitsOnOrderValidator(allowZero: true)]
        public short UnitsOnOrder { get; set; } = 0;
    }
}
