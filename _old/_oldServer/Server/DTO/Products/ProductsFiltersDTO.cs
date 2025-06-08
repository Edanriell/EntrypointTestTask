using Swashbuckle.AspNetCore.Annotations;

using Server.Attributes.Users;
using Server.Attributes.Products;
using Server.Attributes.Orders;

namespace Server.DTO.Products
{
    [SwaggerSchema("Custom filters DTO for products.")]
    public class ProductsFiltersDTO
    {
        [SwaggerParameter("Filter products by code.")]
        public string? Code { get; set; }

        [SwaggerParameter("Filter products by name.")]
        [ProductNameValidator]
        public string? ProductName { get; set; }

        [SwaggerParameter("Filter products by in stock units.")]
        [ProductUnitsInStockValidator(allowZero: true)]
        public short? UnitsInStock { get; set; }

        [SwaggerParameter("Filter products by ordered units .")]
        [ProductUnitsOnOrderValidator(allowZero: true)]
        public short? UnitsOnOrder { get; set; }

        [SwaggerParameter(
            "Filter products by customer name associated with the product through order."
        )]
        [UserNameValidator]
        public string? CustomerName { get; set; }

        [SwaggerParameter(
            "Filter products by the customer surname associated with the product through order."
        )]
        [UserSurnameValidator]
        public string? CustomerSurname { get; set; }

        [SwaggerParameter(
            "Filter products by email address of the customer associated with the product through order."
        )]
        [UserEmailValidator]
        public string? CustomerEmail { get; set; }
    }
}
