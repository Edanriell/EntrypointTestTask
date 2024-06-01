using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

using Server.Attributes.Shared;
using Server.Entities;
using Server.DTO.Products;
using System.ComponentModel.DataAnnotations.Schema;
using Server.Attributes.Products;
using Swashbuckle.AspNetCore.Annotations;
using Server.Shared.Enums;
using Server.Attributes.Users;

namespace Server.DTO.Users
{
    public class CreateUserDTO
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(72)]
        public string Surname { get; set; } = null!;

        [Required]
        [UserUniqueEmailValidator]
        public string Email { get; set; } = null!;

        [Required]
        // [UserUniqueUsernameValidator]
        public string Username { get; set; } = null!;

        [Required]
        [UserUniquePhoneNumberValidator]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MinLength(10), MaxLength(160)]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
