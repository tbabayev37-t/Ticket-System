using Microsoft.AspNetCore.Identity;

namespace TicketSistemi.Models
{
     public sealed class ApplicationUser:IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
