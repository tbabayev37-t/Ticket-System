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
                                                 , ClaimsPrincipal principal, ITicketService ticketService)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            var result = await ticketService.CreateAsync(principal.GetUserId(), request);
            return result.Succeeded
            ? Results.Created($"/tickets/{result.Value}", new { id = result.Value })
            : result.ToProblem();
        }
        private static async Task<IResult> GetMy(ClaimsPrincipal principal, ITicketService ticketService, Models.Enums.SortOrder order= Models.Enums.SortOrder.Desc)
        {
            var list = await ticketService.GetMyTicketsAsync(principal.GetUserId(), order);
            return Results.Ok(list);
        }
        private static async Task<IResult> GetById(Guid id, ClaimsPrincipal principal, 
            ITicketService ticketService,Models.Enums.SortOrder order = Models.Enums.SortOrder.Asc)
        {
            var result = await ticketService.GetByIdAsync(id, principal.GetUserId(), isAdmin: false, order);
            return result.Succeeded ? Results.Ok(result.Value) : result.ToProblem();
        }
        private static async Task<IResult> AddMessage(Guid id, CreateMessageRequest request,
            IValidator<CreateMessageRequest> validator,ClaimsPrincipal principal, ITicketService ticketService)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            var result = await ticketService.AddMessageAsync(id, principal.GetUserId(), isAdmin: false, request);
            return result.Succeeded ? Results.NoContent() : result.ToProblem();
        }
    }
}
