using AppMotor.HttpServer.MvcSample.Services;

using Microsoft.AspNetCore.Mvc;

namespace AppMotor.HttpServer.MvcSample.Controllers;

public sealed class ApiController : Controller
{
    private readonly ServiceSettingsProvider _portService;

    public ApiController(ServiceSettingsProvider portService)
    {
        this._portService = portService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult HttpsPort()
    {
        return Ok(
            new
            {
                HttpsPort = this._portService.HttpsPort,
            }
        );
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Ping()
    {
        return Ok(
            new
            {
                Success = true,
                ServerName = Environment.MachineName,
                RequestReceived = DateTime.UtcNow,
            }
        );
    }
}
