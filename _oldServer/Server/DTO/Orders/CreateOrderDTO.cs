using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

using Server.Attributes.Shared;
using Server.Entities;
using Server.DTO.Products;

namespace Server.DTO.Orders
{
    public class CreateOrderDTO
    {
        [Required]
        [SwaggerParameter("User id.")]
        [IdValidator<User>]
        public int UserId { get; set; }

        [Required]
        [SwaggerParameter("Full ship address.")]
        [StringLength(80)]
        public string ShipAddress { get; set; } = string.Empty;

        [Required]
        [SwaggerParameter("Information about order.")]
        [MaxLength(400)]
        public string OrderInformation { get; set; } = string.Empty;

        [Required]
        [SwaggerParameter("Product ids with quantities.")]
        public List<ProductIdsWithQuantitiesDTO> ProductIdsWithQuantities { get; set; } =
            new List<ProductIdsWithQuantitiesDTO>();
    }
}
