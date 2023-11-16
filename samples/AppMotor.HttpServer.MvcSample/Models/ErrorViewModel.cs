using AppMotor.HttpServer.MvcSample.Controllers;

namespace AppMotor.HttpServer.MvcSample.Models;

/// <summary>
/// Model for <c>Error.cshtml</c> and <see cref="HomeController.Error"/>.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
