using System.ComponentModel.DataAnnotations;

namespace invyoc.Models;

public class CompanyInfo
{
    [Required(ErrorMessage = "Your Company Name is required")]
    public string? Name { get; set; } = string.Empty;
    public AddressInfo? CompanyAddress { get; set; }
    public string? Email { get; set; }
    public string? GSTNo { get; set; }
    public string? LogoBase64 { get; set; }
}