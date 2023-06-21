using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class EditUserRequest : RegisterRequest
{
    [Required]
    public bool Active { get; set; }
    //public string Role { get; set; }
}
