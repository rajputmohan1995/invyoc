using System.ComponentModel.DataAnnotations;

namespace invyoc.Models;

public class ClientInfo
{
    public string? Name { get; set; } = string.Empty;
    public string? GSTNo { get; set; }
    public AddressInfo? ClientAddress { get; set; }   
}