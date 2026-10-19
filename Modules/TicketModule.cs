using Carter;
using FluentValidation;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using TicketSistemi.DTOs.Tickets;
using TicketSistemi.Extensions;
using TicketSistemi.Models;
using TicketSistemi.Services.Abstractions;

namespace TicketSistemi.Modules
{
    public sealed class TicketModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/tickets")
                .WithTags("Tickets")
                .RequireAuthorization(policy => policy.RequireRole(UserRoles.User));

            group.MapPost("/", Create);
            group.MapGet("/my", GetMy);
            group.MapGet("/{id:guid}", GetById);
            group.MapPost("/{id:guid}/messages", AddMessage);


        }
        private static async Task<IResult> Create(CreateTicketRequest request, IValidator<CreateTicketRequest> validator
                                                 , ClaimsPrincipal principal, ITicketService ticketService,CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(request,cancellationToken);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            var result = await ticketService.CreateAsync(principal.GetUserId(), request,cancellationToken);
            return result.Succeeded
            ? Results.Created($"/tickets/{result.Value}", new { id = result.Value })
            : result.ToProblem();
        }
        private static async Task<IResult> GetMy(ClaimsPrincipal principal, ITicketService ticketService, CancellationToken cancellationToken, Models.Enums.SortOrder order= Models.Enums.SortOrder.Desc)
        {
            var list = await ticketService.GetMyTicketsAsync(principal.GetUserId(), order,cancellationToken);
            return Results.Ok(list);
        }
        private static async Task<IResult> GetById(Guid id, ClaimsPrincipal principal, 
            ITicketService ticketService, CancellationToken cancellationToken,Models.Enums.SortOrder order = Models.Enums.SortOrder.Asc)
        {
            var result = await ticketService.GetByIdAsync(id, principal.GetUserId(), isAdmin: false, order,cancellationToken);
            return result.Succeeded ? Results.Ok(result.Value) : result.ToProblem();
        }
        private static async Task<IResult> AddMessage(Guid id, CreateMessageRequest request,
            IValidator<CreateMessageRequest> validator,ClaimsPrincipal principal, ITicketService ticketService, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(request,cancellationToken);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            var result = await ticketService.AddMessageAsync(id, principal.GetUserId(), isAdmin: false, request,cancellationToken);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
    }
}
