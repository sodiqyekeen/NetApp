using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetApp.Domain.Entities;

public class AuthenticationToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(10)]
    public string Key { get; set; } = null!;

    [Required]
    public string Value { get; set; } = null!;
    public DateTime Expires { get; set; }

    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    public bool Expired => Expires < DateTime.UtcNow;
}
