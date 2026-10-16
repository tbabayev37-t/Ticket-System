namespace TicketSistemi.DTOs.Auth
{
    public sealed record RegisterRequestDto(string FullName, 
                                            string Username,
                                            string Email, 
                                            string Password, 
                                            string ConfirmPassword);
}
