namespace Server.DTO.Products
{
    public class ProductDTO : ProductBasicDTO
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public List<ProductOrdersDTO>? ProductOrders { get; set; }
    }
}
