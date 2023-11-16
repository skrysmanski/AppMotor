using AppMotor.CliApp.CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.HostBuilder.Sample;

internal sealed class MyCliCommand : CliCommand
{
    /// <inheritdoc />
    protected override CliCommandExecutor Executor => new(Execute);

    /// <inheritdoc />
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddHostedService<MyTestServer>();
    }

    private void Execute(CancellationToken cancellationToken)
    {
        while (true)
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
