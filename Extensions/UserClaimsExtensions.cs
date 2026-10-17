using System.Security.Claims;
using System.Security.Principal;

namespace TicketSistemi.Extensions
{
    public static class UserClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id)
                ? id
                : throw new InvalidOperationException("User id claim is missing or invalid.");
        }
    }
}