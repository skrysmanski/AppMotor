using AppMotor.HttpServer.Sample.Controllers;

namespace AppMotor.HttpServer.Sample.Models;

/// <summary>
/// Model for <c>Error.cshtml</c> and <see cref="HomeController.Error"/>.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}