using FluentValidation;
using TicketSistemi.DTOs.Tickets;

namespace TicketSistemi.Validator
{
    public sealed class CreateMessageRequestValidator:AbstractValidator<CreateMessageRequest>
    {
        public CreateMessageRequestValidator()
        {
            RuleFor(x=>x.Text).NotEmpty().WithMessage("Message is required").MaximumLength(4000);
        }
    }
}
