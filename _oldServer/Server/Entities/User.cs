using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

// JWT
using Microsoft.AspNetCore.Identity;

using Server.Shared.Interfaces;
using Server.Shared.Enums;

namespace Server.Entities
{
    public enum Gender
    {
        Male,
        Female
    }

    [Table("Users")]
    public class User : IdentityUser<int>, IIdentifiable
    {
        [Key]
        [Required]
        public override int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(72)]
        public string Surname { get; set; } = null!;

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; } = null!;

        //[Required]
        //public string Username { get; set; } = null!;
        public string? Password { get; set; }

        //[StringLength(24)]
        //public string PhoneNumber { get; set; } = null!;

        [Required]
        [MinLength(10), MaxLength(160)]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        [Column(TypeName = "image")]
        public byte[]? Photo { get; set; }

        [Required]
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
