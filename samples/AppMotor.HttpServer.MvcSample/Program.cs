using AppMotor.CliApp.Terminals;
using AppMotor.HttpServer;
using AppMotor.HttpServer.MvcSample.Services;

Terminal.WriteLine("Try the following URLs in your browser:");
Terminal.WriteLine();
Terminal.WriteLine(" - http://localhost:1234");
Terminal.WriteLine(" - http://localhost:1234/api/httpsport");
Terminal.WriteLine(" - http://localhost:1234/api/ping");
Terminal.WriteLine();

var app = new HttpServerApplication(port: 1234);

app.Services.AddSingleton(new ServiceSettingsProvider(httpsPort: 4567));

return app.Run();
