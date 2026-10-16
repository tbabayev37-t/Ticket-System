using FluentValidation;
using TicketSistemi.DTOs.Auth;

namespace TicketSistemi.Validator
{
    public sealed class LoginRequestValidator:AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Username is wrong!");

            RuleFor(x => x.Password).NotEmpty().MinimumLength(5);
        }
    }
}
