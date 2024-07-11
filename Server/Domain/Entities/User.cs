namespace Domain.Entities;

[Table("Users")]
public class User : IdentityUser
{
    [Required] [MaxLength(64)] public string Name { get; set; } = null!;

    [Required] [MaxLength(72)] public string Surname { get; set; } = null!;

    [Required] [MaxLength(24)] public string Password { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(160)]
    public string Address { get; set; } = null!;

    [Required] public DateTime BirthDate { get; set; }

    [Required] public Gender Gender { get; set; }

    [Column(TypeName = "image")] public byte[]? Photo { get; set; }
    [Required] public DateTime CreatedAt { get; set; }

    [Required] public ICollection<Order>? Orders { get; set; } = new List<Order>();
}

// User can have one photo