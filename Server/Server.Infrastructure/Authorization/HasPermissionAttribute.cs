using Microsoft.AspNetCore.Authorization;

namespace Server.Infrastructure.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(
            permission
        )
    {
    }
} 
