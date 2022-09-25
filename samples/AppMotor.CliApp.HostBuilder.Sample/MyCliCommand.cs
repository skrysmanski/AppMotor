using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.HostBuilder.Sample;

internal sealed class MyCliCommand : GenericHostCliCommand
{
    /// <inheritdoc />
    protected override CliCommandExecutor ExplicitExecutor => new(Execute);

    /// <inheritdoc />
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddHostedService<MyTestServer>();
    }

    private void Execute()
    {
        while (!this.LifetimeEvents.Stopping.HasBeenRaised)
        {
            this.Terminal.Write("Enter something: ");
            var text = this.Terminal.ReadLine();

            if (text is null)
            {
                break;
            }
            else if (!string.IsNullOrWhiteSpace(text))
            {
                this.Terminal.WriteLine($"You wrote: {text}");
                break;
            }
        }
    }
}
