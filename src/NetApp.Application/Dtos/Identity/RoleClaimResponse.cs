namespace NetApp.Application.Dtos.Identity;

public class RoleClaimResponse
{
    public int Id { get; set; }
    public string RoleId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Group { get; set; } = null!;
    public bool Selected { get; set; }
}
