using System.ComponentModel.DataAnnotations;

namespace Server.DTO.Products
{
    public class ProductBasicDTO
    {
        [Required]
        public int Id { get; set; }

        public string? ProductName { get; set; }

        public short Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
