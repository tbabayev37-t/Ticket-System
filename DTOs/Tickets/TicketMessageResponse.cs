namespace TicketSistemi.DTOs.Tickets
{
    public sealed record TicketMessageResponse(
    Guid Id,
    string Text,
    DateTimeOffset CreatedAt,
    Guid AuthorId,
    string AuthorName);
}
