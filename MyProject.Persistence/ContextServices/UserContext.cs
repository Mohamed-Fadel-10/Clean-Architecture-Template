using Application.Abstractions.Authentication;
using Domain.Common.Interfaces;
using System.Security.Claims;

namespace Api.ContextServices
{
    /// <summary>
    /// Represents the user context for the application.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UserContext"/> class.
    /// </remarks>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public class UserContext(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        /// <summary>
        /// Gets the current user's ID from the HTTP context claims.
        /// </summary>
        public Guid? CurrentUserId
        {
            get
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    User?.FindFirstValue("user-id");
                return Guid.TryParse(userId, out var id) ? id : null;
            }
        }

        /// <summary>
        /// Indicates whether soft delete is enabled for the current user context.
        /// </summary>
        public bool SoftDeleteEnabled { get; set; } = true;
        public string? Email =>
            User?.FindFirstValue(ClaimTypes.Email) ??
            User?.FindFirstValue("email");
        /// <summary>
        /// Gets the current user's email from the HTTP context claims.
        /// </summary>
        public string CurrentUserName =>
                    User?.FindFirstValue(ClaimTypes.Name) ??
                    User?.FindFirstValue("full-name") ??
                    Email ??
                    string.Empty;

        public bool IsAuthenticated =>
                    User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return
                User?.IsInRole(role) == true ||
                User?.Claims.Any(c => c.Type == "role" && c.Value == role) == true;
        }
    }
}
