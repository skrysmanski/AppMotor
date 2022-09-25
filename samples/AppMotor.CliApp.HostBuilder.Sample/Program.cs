using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.HostBuilder.Sample;

var command = new MyCliCommand();
var app = new CliApplicationWithCommand(command);

return app.Run();
