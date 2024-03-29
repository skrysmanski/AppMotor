﻿using System.Diagnostics;

using AppMotor.HttpServer.MvcSample.Models;

using Microsoft.AspNetCore.Mvc;

namespace AppMotor.HttpServer.MvcSample.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
            }
        );
    }
}
