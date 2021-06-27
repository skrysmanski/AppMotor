using AppMotor.CliApp.CommandLine.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.Hosting.Sample
{
    internal sealed class MyGenericHostCliCommand : GenericHostCliCommand
    {
        /// <inheritdoc />
        protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<MyTestServer>();
        }
    }
}
