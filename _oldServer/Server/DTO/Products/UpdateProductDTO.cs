using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

using Server.Entities;
using Server.Attributes.Shared;
using Server.Attributes.Products;

namespace Server.DTO.Products
{
    public class UpdateProductDTO
    {
        [Required]
        [SwaggerParameter("Product id.")]
        [IdValidator<Product>]
        public int Id { get; set; }

        [SwaggerParameter("Product code.")]
        public string Code { get; set; } = null!;

        [SwaggerParameter("Product name.")]
        [UniqueProductNameValidator]
        public string ProductName { get; set; } = null!;

        [SwaggerParameter("Product description.")]
        [MaxLength(300)]
        public string Description { get; set; } = null!;

        [SwaggerParameter("Product unit price.")]
        [ProductUnitPriceValidator]
        public decimal UnitPrice { get; set; }

        [SwaggerParameter("Product units in stock.")]
        [ProductUnitsInStockValidator]
        public short UnitsInStock { get; set; }

        [SwaggerParameter("Product units on order.")]
        [ProductUnitsOnOrderValidator(allowZero: true)]
        public short UnitsOnOrder { get; set; } = 0;
    }
}
