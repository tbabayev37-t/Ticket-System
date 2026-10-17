using TicketSistemi.Models.Common;
using TicketSistemi.Models.Enums;

namespace TicketSistemi.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToProblem(this Result result) => result.ErrorType switch
        {
            ErrorType.NotFound => Results.NotFound(new { errors = result.Errors }),
            ErrorType.Conflict => Results.Conflict(new { errors = result.Errors }),
            _ => Results.BadRequest(new { errors = result.Errors })
        };
    }
}
