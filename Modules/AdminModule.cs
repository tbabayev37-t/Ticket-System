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
            
        }
        private static async Task<IResult> GetAll(ITicketService ticketService, TicketStatus? status = null,SortOrder order = SortOrder.Desc)
        {
            var list = await ticketService.GetAllAsync(status, order);
            return Results.Ok(list);
        }
        private static async Task<IResult>GetById(Guid id, ClaimsPrincipal principal,
        ITicketService tickets,
        SortOrder order = SortOrder.Asc)
        {
            var result = await tickets.GetByIdAsync(id, principal.GetUserId(), isAdmin: true, order);
            return result.Succeeded ? Results.Ok(result.Value) : result.ToProblem();
        }
        private static async Task<IResult> Reply(Guid id, CreateMessageRequest request, IValidator<CreateMessageRequest>validator,
        ClaimsPrincipal principal, ITicketService ticketService)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            var result = await ticketService.AddMessageAsync(id, principal.GetUserId(), isAdmin: true, request);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
        private static async Task<IResult> Close(Guid id, ITicketService ticketService)
        {
            var result = await ticketService.CloseAsync(id);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
    }
}
