using TicketSistemi.DTOs.Tickets;
using TicketSistemi.Models.Common;
using TicketSistemi.Models.Enums;

namespace TicketSistemi.Services.Abstractions
{
    public interface ITicketService
    {
        Task<Result<Guid>> CreateAsync(Guid userId, CreateTicketRequest request, CancellationToken cancellationToken = default);
        Task<List<TicketSummaryResponse>> GetMyTicketsAsync(Guid userId, Models.Enums.SortOrder order, CancellationToken cancellationToken = default);
        Task<Result<TicketDetailResponse>> GetByIdAsync(Guid ticketId, Guid userId, bool isAdmin, Models.Enums.SortOrder order, CancellationToken cancellationToken = default);
        Task<Result> AddMessageAsync(Guid ticketId, Guid userId, bool isAdmin, CreateMessageRequest request, CancellationToken cancellationToken = default);

        //admin
        Task<List<TicketSummaryResponse>> GetAllAsync(TicketStatus? status, Models.Enums.SortOrder order, CancellationToken cancellationToken = default);
        Task<Result> CloseAsync(Guid ticketId, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default);

    }
}
