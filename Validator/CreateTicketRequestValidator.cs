using FluentValidation;
using TicketSistemi.DTOs.Tickets;

namespace TicketSistemi.Validator
{
    public sealed class CreateTicketRequestValidator:AbstractValidator<CreateTicketRequest>
    {
        public CreateTicketRequestValidator()
        {
            RuleFor(x=>x.Title).NotEmpty().MinimumLength(2);
            RuleFor(x=>x.Description).NotEmpty().WithMessage("Message is required").MaximumLength(2000);
        }
    }
}
