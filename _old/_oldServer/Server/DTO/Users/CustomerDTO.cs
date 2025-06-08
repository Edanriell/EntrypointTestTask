using System.ComponentModel.DataAnnotations;

using Server.DTO.Orders;
using Server.Entities;

namespace Server.DTO.Users
{
    public class CustomerDTO : UserDTO
    {
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<OrderDTO>? Orders { get; set; }
    }
}
