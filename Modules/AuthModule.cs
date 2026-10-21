using Carter;
using FluentValidation;
using TicketSistemi.DTOs.Auth;
using TicketSistemi.Services.Abstractions;

namespace TicketSistemi.Modules
{
    public sealed class AuthModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth").RequireRateLimiting("FixedWindowLimiter");
            group.MapPost("/register", Register);
            group.MapPost("/login", Login);
        }
        private static async Task<IResult> Register(RegisterRequestDto request, 
                                                    IValidator<RegisterRequestDto> validator,
                                                    IAuthService authSerice)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            var result = await authSerice.RegisterAsync(request);
            return result.Succeeded ? Results.Ok(new {message = "Registrated successfully"})
                                    : Results.BadRequest(new {errors = result.Errors});

        }
        private static async Task<IResult> Login(LoginRequestDto request,
                                                IValidator<LoginRequestDto> validator,
                                                IAuthService authService)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            var result = await authService.LoginAsync(request);
            return result.Succeeded ? Results.Ok(result.Value)
                                    : Results.Json(new {errors = result.Errors},
                                    statusCode: StatusCodes.Status401Unauthorized);
        }
    }
}
