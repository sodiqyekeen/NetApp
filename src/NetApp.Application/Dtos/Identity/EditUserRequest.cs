using System.ComponentModel.DataAnnotations;

namespace NetApp.Application.Dtos.Identity;

public class EditUserRequest : RegisterRequest
{
    [Required]
    public bool Active { get; set; }
    //public string Role { get; set; }
}
