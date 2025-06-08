using System.ComponentModel.DataAnnotations;

namespace Server.Entities
{
    public class ProductOrderLink
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        [Key]
        [Required]
        public int ProductId { get; set; }

        [Required]
        public short Quantity { get; set; }

        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}
