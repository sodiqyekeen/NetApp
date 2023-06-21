namespace NetApp.Dtos;

public record EmailRequest(
        string To,
        string Subject,
        string Body,
        string? From = null
);
