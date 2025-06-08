using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

using Server.Attributes.Shared;
using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.DTO.Orders
{
    public class DeleteOrderDTO
    {
        [Required]
        [SwaggerParameter("Unique order id.")]
        [IdValidator<Order>]
        public int Id { get; set; }
    }
}
