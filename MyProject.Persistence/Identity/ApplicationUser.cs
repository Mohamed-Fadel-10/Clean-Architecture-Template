using Microsoft.AspNetCore.Identity;

namespace MyProject.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;

        public Guid DomainUserId { get; set; }
    }
}