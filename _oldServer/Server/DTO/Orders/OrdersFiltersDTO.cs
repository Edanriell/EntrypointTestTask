using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

using Server.Shared.Enums;
using Server.Attributes.Users;
using Server.Attributes.Products;
using Server.Attributes.Orders;

namespace Server.DTO.Orders
{
    [SwaggerSchema("Custom filters DTO for orders.")]
    public class OrdersFiltersDTO
    {
        [SwaggerParameter("Order status.")]
        public OrderStatus? Status { get; set; }

        [SwaggerParameter("Full ship address.")]
        [ShipAddressValidator]
        public string? ShipAddress { get; set; }

        [UserNameValidator]
        [SwaggerParameter("Order, user name.")]
        public string? UserName { get; set; }

        [UserSurnameValidator]
        [SwaggerParameter("Order, user surname.")]
        public string? UserSurname { get; set; }

        [UserEmailValidator]
        [SwaggerParameter("Order, user email.")]
        public string? UserEmail { get; set; }

        [ProductNameValidator]
        [SwaggerParameter("Order, product name.")]
        public string? ProductName { get; set; }
    }
}
