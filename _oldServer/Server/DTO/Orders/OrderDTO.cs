using System.ComponentModel.DataAnnotations;

using Server.Shared.Enums;
using Server.DTO.Products;
using Server.DTO.Users;

namespace Server.DTO.Orders
{
    public class OrderDTO
    {
        [Required]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public OrderStatus Status { get; set; }

        public string? ShipAddress { get; set; }

        public string? OrderInformation { get; set; }

        public CustomerDTO? Customer { get; set; }

        public ICollection<ProductBasicDTO>? Products { get; set; }
    }
}
