using NetApp.Domain.Exceptions;

namespace NetApp.Domain.Common;

public static class Guard
{
    public static void AgainstNull(object? value, string errorMessage)
    {
        if (value != null) return;
        throw new NotFoundException(errorMessage);
    }
}
