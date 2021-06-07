using System;

using AppMotor.HttpServer.Sample.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppMotor.HttpServer.Sample.Controllers
{
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
            // TODO: Only allow this via HTTPS
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
}
