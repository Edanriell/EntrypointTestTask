using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

using Server.Entities;
using Server.Shared.Enums;
using Server.Attributes.Shared;

namespace Server.DTO.Orders
{
    public class UpdateOrderDTO
    {
        [Required]
        [SwaggerParameter("Order id.")]
        [IdValidator<Order>]
        public int Id { get; set; }

        [SwaggerParameter("Order status.")]
        public OrderStatus Status { get; set; }

        [MaxLength(400)]
        [SwaggerParameter("Information about order.")]
        public string? OrderInformation { get; set; }
    }
}
