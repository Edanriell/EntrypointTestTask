using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

using Server.Attributes.Shared;
using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.DTO.Products
{
    public class DeleteProductDTO
    {
        [Required]
        [SwaggerParameter("Unique product id.")]
        [IdValidator<Product>]
        public int Id { get; set; }
    }
}
