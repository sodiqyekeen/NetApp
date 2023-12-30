namespace NetApp.Dtos;

public record EmailRequest(
        string To,
        string TemplateName,
        object? ReplacementValues = null,
        string? From = null
);

