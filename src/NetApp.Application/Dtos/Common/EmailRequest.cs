namespace NetApp.Application.Dtos.Common;

public record EmailRequest(
        string To,
        string Subject,
        string Body,
        string? From = null
);
