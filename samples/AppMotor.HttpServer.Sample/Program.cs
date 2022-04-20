using AppMotor.CliApp.Terminals;
using AppMotor.HttpServer;
using AppMotor.HttpServer.Sample.Services;

Terminal.WriteLine("Try the following URLs:");
Terminal.WriteLine();
Terminal.WriteLine(" - http://localhost:1234/api/httpsport");
Terminal.WriteLine(" - http://localhost:1234/api/ping");
Terminal.WriteLine();

var app = new HttpServerApplication(port: 1234);

app.Services.AddSingleton(new ServiceSettingsProvider(4567));

return app.Run();
