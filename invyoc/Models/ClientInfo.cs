using System.ComponentModel.DataAnnotations;

namespace invyoc.Models;

public class ClientInfo
{
    public string? Name { get; set; } = string.Empty;
    public string? GSTNo { get; set; }
    public AddressInfo? ClientAddress { get; set; }
}

public class ClientInfoRequired : ClientInfo
{
    [Required(ErrorMessage = "Client Company/Name is required")]
    public new string? Name
    {
        get => base.Name;
        set => base.Name = value;
    }
}