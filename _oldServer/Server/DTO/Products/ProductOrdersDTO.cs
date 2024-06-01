using Server.Shared.Enums;

namespace Server.DTO.Products
{
    public class ProductOrdersDTO
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerSurname { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerAddress { get; set; }
        public short ProductOrderQuantity { get; set; }
    }
}
