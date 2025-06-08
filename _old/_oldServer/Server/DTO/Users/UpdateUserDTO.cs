using Server.Attributes.Shared;
using Server.Attributes.Users;
using Server.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Server.DTO.Users
{
    public class UpdateUserDTO
    {
        [Required]
        [SwaggerParameter("Unique user id.")]
        [IdValidator<User>]
        public int Id { get; set; }

        [MaxLength(64)]
        public string Name { get; set; } = null!;

        [MaxLength(72)]
        public string Surname { get; set; } = null!;

        [UserUniqueEmailValidator]
        public string Email { get; set; } = null!;

        // [UserUniqueUsernameValidator]
        public string Username { get; set; } = null!;

        [UserUniquePhoneNumberValidator]
        public string PhoneNumber { get; set; } = null!;

        [MinLength(10), MaxLength(160)]
        public string Address { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
