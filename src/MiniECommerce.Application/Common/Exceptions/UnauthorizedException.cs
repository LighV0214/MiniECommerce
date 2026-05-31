namespace MiniECommerce.Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("Authentication is required to access this resource.")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
