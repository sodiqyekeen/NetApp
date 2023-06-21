using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class RoleClaimRequest
{
    public int? Id { get; set; }
    [Required] public string? RoleId { get; set; }
    [Required] public string? Value { get; set; }
    [Required] public string? Group { get; set; }
    public bool Selected { get; set; }
}
