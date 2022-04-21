using AppMotor.CliApp.Terminals;
using AppMotor.HttpServer;
using AppMotor.HttpServer.Startups;
using AppMotor.HttpServer.WebApiSample.Db;

using Microsoft.EntityFrameworkCore;

Terminal.WriteLine("The web api controller is available under:");
Terminal.WriteLine();
Terminal.WriteLine(" http://localhost:1234/api/todoitems");
Terminal.WriteLine();
Terminal.WriteLine("See this documentation for an easy way of how to work with this api:");
Terminal.WriteLine();
Terminal.WriteLine("  https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?#install-http-repl");
Terminal.WriteLine();

var app = new HttpServerApplication(port: 1234, startupClass: new WebApiStartup());

app.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

return app.Run();
