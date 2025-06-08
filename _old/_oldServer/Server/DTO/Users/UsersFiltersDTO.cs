using Swashbuckle.AspNetCore.Annotations;

using Server.Attributes.Users;
using Server.Attributes.Products;
using Server.Entities;
using Server.Shared.Enums;

namespace Server.DTO.Users
{
    [SwaggerSchema("Custom filters DTO for users.")]
    public class UsersFiltersDTO
    {
        [SwaggerParameter("Filter users by they role.")]
        public UserRole? Role { get; set; }

        [SwaggerParameter("Filter users by they name.")]
        [UserNameValidator]
        public string? Name { get; set; }

        [SwaggerParameter("Filter users by they surname.")]
        [UserSurnameValidator]
        public string? Surname { get; set; }

        [SwaggerParameter("Filter users by they email.")]
        [UserEmailValidator]
        public string? Email { get; set; }

        [SwaggerParameter("Filter users by they username.")]
        [UserUsernameValidator]
        public string? Username { get; set; }

        [SwaggerParameter("Filter users by they gender.")]
        public Gender? Gender { get; set; }

        [SwaggerParameter("Filter users by order creation date.")]
        public DateTime? CreatedAt { get; set; }

        [SwaggerParameter("Filter users by order last update date.")]
        public DateTime? UpdatedAt { get; set; }

        [SwaggerParameter("Filter users by order status.")]
        public OrderStatus? Status { get; set; }

        [SwaggerParameter("Filter users by ordered product code.")]
        [ProductCodeValidator]
        public string? ProductCode { get; set; }

        [SwaggerParameter("Filter users by ordered product name.")]
        [ProductNameValidator]
        public string? ProductName { get; set; }
    }
}
