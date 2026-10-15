namespace TicketSistemi.DTOs.Auth
{
    public sealed record AuthResponse(string Token, DateTimeOffset ExpiresAt);
}
