namespace TicketSistemi.Models.Common
{
    public class Result
    {
        public bool Succeeded {  get; set; }
        public IReadOnlyList<string> Errors { get; set; } = [];

        public static Result Ok() => new() { Succeeded = true };
        public static Result Fail(params string[] errors) => new() {Errors = errors};
        public static Result Fail(IEnumerable<string> errors) => new() { Errors = errors.ToList()};
    }
    public sealed class Result<T> : Result
    {
        public T? Value { get; set; }
        public static Result<T> Ok(T value) => new() {Succeeded = true, Value = value };
        public new static Result<T> Fail(params string[] errors) => new() {Succeeded = false, Errors = errors };

    }
}
