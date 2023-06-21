namespace NetApp.Dtos;

public class PermissionDto
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Selected { get; set; }
}
