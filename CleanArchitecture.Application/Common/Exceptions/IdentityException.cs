namespace CleanArchitecture.Application.Common.Exceptions;

public class IdentityException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public IdentityException(IEnumerable<string> errors)
        : base(string.Join("; ", errors))
    {
        Errors = errors.ToList();
    }
}
