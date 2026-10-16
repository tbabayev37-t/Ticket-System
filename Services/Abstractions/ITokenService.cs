using TicketSistemi.DTOs.Auth;
using TicketSistemi.Models;

namespace TicketSistemi.Services.Abstractions
{
    public interface ITokenService
    { 
        AuthResponse CreateToken(ApplicationUser user, IList<string> roles);
    }
}
