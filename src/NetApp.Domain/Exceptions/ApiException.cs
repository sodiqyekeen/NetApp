namespace NetApp.Domain.Exceptions;
public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {

    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message)
    {

    }
}

public class InvalidEmailAddressException : ApiException
{
    public InvalidEmailAddressException(string email) : base($"Invalid email address '{email}'.") { }
}