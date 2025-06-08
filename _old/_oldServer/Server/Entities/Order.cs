using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using Server.Shared.Interfaces;
using Server.Shared.Enums;

namespace Server.Entities
{
    [Table("Orders")]
    public class Order : IIdentifiable
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = new DateTime();

        [Required]
        public DateTime UpdatedAt { get; set; } = new DateTime();

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Created;

        [Required]
        [StringLength(80)]
        public string ShipAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(400)]
        public string OrderInformation { get; set; } = string.Empty;

        public User? User { get; set; }

        public ICollection<ProductOrderLink>? OrderProducts { get; set; }
    }
}
