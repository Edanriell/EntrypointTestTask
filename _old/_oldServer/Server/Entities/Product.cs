using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using Server.Shared.Interfaces;

namespace Server.Entities
{
    [Table("Products")]
    public class Product : IIdentifiable
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string Description { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public short UnitsInStock { get; set; }

        [Required]
        public short UnitsOnOrder { get; set; } = 0;
        public ICollection<ProductOrderLink>? ProductOrders { get; set; }
    }
}
