using TicketSistemi.Models.Enums;

namespace TicketSistemi.DTOs.Tickets
{
    public sealed record TicketSummaryResponse(
    Guid Id,
    string Title,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ClosedAt,
    string CreatedByName);
}
