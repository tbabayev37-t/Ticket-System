using Carter;
using FluentValidation;
using System.Security.Claims;
using TicketSistemi.DTOs.Tickets;
using TicketSistemi.Extensions;
using TicketSistemi.Models;
using TicketSistemi.Models.Enums;
using TicketSistemi.Services.Abstractions;

namespace TicketSistemi.Modules
{
    public sealed class AdminModule : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/admin/tickets")
                .WithTags("Admin")
                .RequireAuthorization(policy => policy.RequireRole(UserRoles.Admin));

            group.MapGet("/", GetAll);
            group.MapGet("/{id:guid}", GetById);
            group.MapPost("/{id:guid}/messages", Reply);
            group.MapPost("/{id:guid}/close", Close);
            group.MapDelete("/{id:guid}", Delete);
            
        }
        private static async Task<IResult> GetAll(ITicketService ticketService, CancellationToken cancellationToken, TicketStatus? status = null,SortOrder order = SortOrder.Desc)
        {
            var list = await ticketService.GetAllAsync(status, order,cancellationToken);
            return Results.Ok(list);
        }
        private static async Task<IResult>GetById(Guid id, ClaimsPrincipal principal,
        ITicketService tickets, CancellationToken cancellationToken,
        SortOrder order = SortOrder.Asc)
        {
            var result = await tickets.GetByIdAsync(id, principal.GetUserId(), isAdmin: true, order, cancellationToken);
            return result.Succeeded ? Results.Ok(result.Value) : result.ToProblem();
        }
        private static async Task<IResult> Reply(Guid id, CreateMessageRequest request, IValidator<CreateMessageRequest>validator,
        ClaimsPrincipal principal, ITicketService ticketService, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(request,cancellationToken);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            var result = await ticketService.AddMessageAsync(id, principal.GetUserId(), isAdmin: true, request,cancellationToken);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
        private static async Task<IResult> Close(Guid id, ITicketService ticketService, CancellationToken cancellationToken)
        {
            var result = await ticketService.CloseAsync(id,cancellationToken);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
        private static async Task<IResult> Delete(Guid id, ITicketService ticketService, CancellationToken cancellationToken)
        {
            var result = await ticketService.DeleteAsync(id,cancellationToken);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
    }
}
