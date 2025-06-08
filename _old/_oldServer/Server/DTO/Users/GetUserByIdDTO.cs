using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

using Server.Attributes.Shared;
using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.DTO.Users
{
    public class GetUserByIdDTO
    {
        [Required]
        [SwaggerParameter("Unique user id.")]
        [IdValidator<User>]
        public int Id { get; set; }
    }
}
