using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

using Server.Entities;
using Server.Attributes.Shared;
using Server.Attributes.Orders;

namespace Server.DTO.Products
{
    public class ProductIdsWithQuantitiesDTO
    {
        [Required]
        [SwaggerParameter("Product id.")]
        [IdValidator<Product>]
        public int ProductId { get; set; }

        [Required]
        [SwaggerParameter("Quantity of the product.")]
        [OrderQuantityValidator]
        public short Quantity { get; set; } = short.MinValue;
    }
}
