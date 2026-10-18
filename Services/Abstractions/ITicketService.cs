using TicketSistemi.DTOs.Tickets;
using TicketSistemi.Models.Common;
using TicketSistemi.Models.Enums;

namespace TicketSistemi.Services.Abstractions
{
    public interface ITicketService
    {
        Task<Result<Guid>> CreateAsync(Guid userId, CreateTicketRequest request);
        Task<List<TicketSummaryResponse>> GetMyTicketsAsync(Guid userId, Models.Enums.SortOrder order);
        Task<Result<TicketDetailResponse>> GetByIdAsync(Guid ticketId, Guid userId, bool isAdmin, Models.Enums.SortOrder order);
        Task<Result> AddMessageAsync(Guid ticketId, Guid userId, bool isAdmin, CreateMessageRequest request);

        //admin
        Task<List<TicketSummaryResponse>> GetAllAsync(TicketStatus? status, Models.Enums.SortOrder order);
        Task<Result> CloseAsync(Guid ticketId);
        Task<Result> DeleteAsync(Guid ticketId);

    }
}
