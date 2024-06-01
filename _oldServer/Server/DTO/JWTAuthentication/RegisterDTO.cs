using Server.Attributes.Users;
using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.DTO.JWTAuthentication
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(72)]
        public string Surname { get; set; } = null!;

        //[Required]
        //[UserUniqueEmailValidator]
        //public string Email { get; set; } = null!;

        //[Required]
        //// [UserUniqueUsernameValidator]
        //public string Username { get; set; } = null!;

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
