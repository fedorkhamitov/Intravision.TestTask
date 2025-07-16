namespace Intravision.TestTask.Domain.Exceptions;

public class DomainExceptions : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}