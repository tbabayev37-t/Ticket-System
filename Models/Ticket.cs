using TicketSistemi.Models.Enums;

namespace TicketSistemi.Models
{
    public sealed class Ticket
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public DateTimeOffset CreatedAt {  get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ClosedAt { get; set; }

        public Guid CreatByUserId { get; set; }
        public ApplicationUser CreatByUser { get; set; } = default!;

        public List<TicketMessage> Messages { get; set; } = new();
    }
}