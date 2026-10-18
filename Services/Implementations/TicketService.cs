using Microsoft.EntityFrameworkCore;
using TicketSistemi.Context;
using TicketSistemi.DTOs.Tickets;
using TicketSistemi.Models;
using TicketSistemi.Models.Common;
using TicketSistemi.Models.Enums;
using TicketSistemi.Services.Abstractions;

namespace TicketSistemi.Services.Implementations
{
    public sealed class TicketService(ApplicationDbContext db) : ITicketService
    {
        public async Task<Result> AddMessageAsync(Guid ticketId, Guid userId, bool isAdmin, CreateMessageRequest request)
        {
            var ticket = await db.Tickets.FirstOrDefaultAsync(t=>t.Id == ticketId
                                                              && (isAdmin || t.CreatByUserId == userId));
            if (ticket is null)
                return Result.NotFound("Ticket not found.");

            if (ticket.Status == TicketStatus.Closed)
                return Result.Conflict("Ticket is closed.");

            db.TicketMessages.Add(new TicketMessage
            {
                TicketId = ticketId,
                AuthorId = userId,
                Text = request.Text.Trim()
            });

            if(isAdmin && ticket.Status == TicketStatus.Open)
            {
                ticket.Status = TicketStatus.InProgress;
            }
            await db.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> CloseAsync(Guid ticketId)
        {
            var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket is null)
                return Result.NotFound("Ticket not found.");

            if (ticket.Status == TicketStatus.Closed)
                return Result.Conflict("Ticket is already closed.");

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result<Guid>> CreateAsync(Guid userId, CreateTicketRequest request)
        {
            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                CreatByUserId = userId
            };
            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();
            return Result<Guid>.Ok(ticket.Id);
        }

        public async Task<List<TicketSummaryResponse>> GetAllAsync(TicketStatus? status, SortOrder order)
        {
            var query = db.Tickets.AsNoTracking();
            if (status is not null)
            {
                query = query.Where(t=>t.Status == status);
            }
            return await ToSummaries(query, order).ToListAsync();
        }

        public async Task<Result<TicketDetailResponse>> GetByIdAsync(Guid ticketId, Guid userId, bool isAdmin, SortOrder order)
        {
            var ticket = await db.Tickets.AsNoTracking().Where(t => t.Id == ticketId && (isAdmin || t.CreatByUserId == userId))
                               .Select(t => new
                               {
                                   t.Id,
                                   t.Title,
                                   t.Description,
                                   t.Status,
                                   t.CreatedAt,
                                   t.ClosedAt,
                                   t.CreatByUserId,
                                   CreatedByName = t.CreatByUser.FullName
                               }).FirstOrDefaultAsync();

            if (ticket is null)
                return Result<TicketDetailResponse>.NotFound("Ticket not found.");

            var messagesQuery = db.TicketMessages.AsNoTracking().Where(m => m.TicketId == ticketId);
            messagesQuery = order == SortOrder.Asc
                ? messagesQuery.OrderBy(m => m.CreatedAt)
                : messagesQuery.OrderByDescending(m => m.CreatedAt);

            var messages = await messagesQuery
                .Select(m => new TicketMessageResponse(
                    m.Id, m.Text, m.CreatedAt, m.AuthorId, m.Author.FullName))
                .ToListAsync();

            return Result<TicketDetailResponse>.Ok(new TicketDetailResponse(
                ticket.Id, ticket.Title, ticket.Description, ticket.Status,
                ticket.CreatedAt, ticket.ClosedAt,
                ticket.CreatByUserId, ticket.CreatedByName, messages));
        }
        public async Task<Result> DeleteAsync(Guid ticketId)
        {
            var ticket =  await db.Tickets.FirstOrDefaultAsync(m => m.Id == ticketId);
            if(ticket is null)
            {
                return Result.NotFound("Ticket not found");
            }
            if (ticket.Status != TicketStatus.Closed)
                return Result.Conflict("Only closed tickets can be deleted.");
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<List<TicketSummaryResponse>> GetMyTicketsAsync(Guid userId, SortOrder order)
        {
            var query = db.Tickets.AsNoTracking().Where(t => t.CreatByUserId == userId);
            return await ToSummaries(query, order).ToListAsync();
        }
        private static IQueryable<TicketSummaryResponse> ToSummaries(IQueryable<Ticket> query, SortOrder order)
        {
            var ordered = order == SortOrder.Asc
                ? query.OrderBy(t => t.CreatedAt)
                : query.OrderByDescending(t => t.CreatedAt);

            return ordered.Select(t => new TicketSummaryResponse(
                t.Id, t.Title, t.Status, t.CreatedAt, t.ClosedAt, t.CreatByUser.FullName));
        }
    }
}
