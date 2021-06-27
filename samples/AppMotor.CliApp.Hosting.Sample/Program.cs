using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Hosting.Sample;

var command = new MyGenericHostCliCommand();
var app = new CliApplicationWithCommand(command);

return app.Run();
