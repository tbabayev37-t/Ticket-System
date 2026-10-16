using TicketSistemi.DTOs.Auth;
using TicketSistemi.Models.Common;

namespace TicketSistemi.Services.Abstractions
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequestDto registerRequest);
        Task<Result<AuthResponse>> LoginAsync(LoginRequestDto loginRequest);
    }
}
