using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TicketSistemi.DTOs.Auth;
using TicketSistemi.Models;
using TicketSistemi.Models.Common;
using TicketSistemi.Models.Enums;
using TicketSistemi.Services.Abstractions;

namespace TicketSistemi.Services.Implementations
{
    public sealed class AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService) : IAuthService
    {
        private const string InvalidCredentials = "Invalid username or password!";
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await userManager.FindByNameAsync(loginRequest.UserName);
            if(user is null)
            {
                return Result<AuthResponse>.Fail(InvalidCredentials);
            }
            if(await userManager.IsLockedOutAsync(user))
            {
                return Result<AuthResponse>.Fail("Account is temporarily locked. Try again later");
            }
            if(!await userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                await userManager.AccessFailedAsync(user);   //lockout saygaci
                return Result<AuthResponse>.Fail(InvalidCredentials);
            }

            await userManager.ResetAccessFailedCountAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.CreateToken(user, roles);
            return Result<AuthResponse>.Ok(token);
        }

        public async Task<Result> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var user = new ApplicationUser
            {
                FullName = registerRequest.FullName,
                UserName = registerRequest.Username,
                Email = registerRequest.Email
            };
            var createResult = await userManager.CreateAsync(user, registerRequest.Password);
            if (!createResult.Succeeded)
            {
                return Result.Fail(createResult.Errors.Select(e => e.Description));
            }
            var roleResult = await userManager.AddToRoleAsync(user, UserRoles.User);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);   //rolsuz user olmasin deye silirik
                return Result.Fail(roleResult.Errors.Select(e=>e.Description));
            }
            return Result.Ok();
        }
    }
}
