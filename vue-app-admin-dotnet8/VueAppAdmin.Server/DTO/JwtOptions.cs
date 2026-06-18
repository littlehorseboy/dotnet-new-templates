using System.ComponentModel.DataAnnotations;

namespace VueAppAdmin.Server.DTO;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string SignKey { get; set; } = string.Empty;
}
