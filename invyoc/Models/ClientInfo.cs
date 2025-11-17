using System.ComponentModel.DataAnnotations;

namespace invyoc.Models;

public class ClientInfo
{
    [Required(ErrorMessage = "Client Company Name is required")]
    public string? Name { get; set; } = string.Empty;
    public string? GSTNo { get; set; }
    public AddressInfo? ClientAddress { get; set; }   
}