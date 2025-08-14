using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AzureDevNexus.Server.Services;

namespace AzureDevNexus.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorization = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authorization.Substring("Bearer ".Length);
            var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();
            var principal = jwtService.ValidateToken(token);

            if (principal == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if role-based authorization is required
            if (_roles.Length > 0)
            {
                var userRole = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                
                if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            // Store user information in HttpContext for use in controllers
            var userId = principal.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                context.HttpContext.Items["UserId"] = userId;
            }

            var userName = principal.FindFirst("UserName")?.Value;
            if (!string.IsNullOrEmpty(userName))
            {
                context.HttpContext.Items["UserName"] = userName;
            }
        }
    }
}
