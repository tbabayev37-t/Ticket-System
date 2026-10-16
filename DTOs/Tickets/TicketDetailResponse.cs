using TicketSistemi.Models.Enums;

namespace TicketSistemi.DTOs.Tickets
{
    public sealed record TicketDetailResponse(
    Guid Id,
    string Title,
    string Description,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ClosedAt,
    Guid CreatedByUserId,
    string CreatedByName,
    List<TicketMessageResponse> Messages);
}
