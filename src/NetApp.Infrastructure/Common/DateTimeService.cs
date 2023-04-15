using NetApp.Application.Common;

namespace NetApp.Infrastructure.Common;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.UtcNow;
}
