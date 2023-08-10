using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoJueves.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

[NotMapped]
public class Errors {
    public bool NonError { get; set; }
    public string? Msj { get; set; }
}
