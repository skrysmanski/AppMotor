using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.Hosting.Sample
{
    internal sealed class MyGenericHostCliCommand : GenericHostCliCommand
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
            this.Terminal.Write("Enter something: ");
            var text = this.Terminal.ReadLine();
            this.Terminal.WriteLine($"You wrote: {text}");
        }
    }
}
