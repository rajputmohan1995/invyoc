using System.ComponentModel.DataAnnotations;

namespace invyoc.Models;

public class ClientInfo
{
    [Required(ErrorMessage = "Client Company Name is required")]
    public string? Name { get; set; } = string.Empty;
    public string? GSTNo { get; set; }
    public string? ContactNum { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}