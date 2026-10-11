namespace TicketSistemi.Models
{
    public sealed class TicketMessage
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid TicketId {  get; set; }
        public Ticket Ticket { get; set; } = default!;

        public Guid AuthorId {  get; set; }
        public ApplicationUser Author { get; set; } = default!;
    }
}
