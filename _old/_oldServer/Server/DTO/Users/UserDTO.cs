using Server.Entities;
using Server.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Server.DTO.Users
{
    public class UserDTO
    {
        [Required]
        public int Id { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public byte[]? Photo { get; set; }
    }
}
