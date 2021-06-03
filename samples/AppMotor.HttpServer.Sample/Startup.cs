using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.HttpServer.Sample
{
    /// <summary>
    /// Startup class for this ASP.NET Core application. Specified by <see cref="Program.SampleServerCommand.StartupClass"/>.
    /// </summary>
    /// <remarks>
    /// Both the constructor and <see cref="Configure"/> support dependency injection for their parameters.
    /// This is why this class has no interface for its methods.
    ///
    /// <para>See also: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup </para>
    /// </remarks>
    public sealed class Startup
    {
        /// <summary>
        /// This method gets called by the ASP.NET Core runtime. It registers services in the dependency injection
        /// system exposed via <paramref name="services"/>.
        /// </summary>
        /// <remarks>
        /// The name of this method is pre-defined and must not be changed.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        /// <summary>
        /// This method gets called by the ASP.NET Core runtime. It creates the ASP.NET Core Middleware
        /// pipeline.
        /// </summary>
        /// <remarks>
        /// You can request any registered service as parameter in this method. Parameters are provided
        /// by the dependency injection framework.
        ///
        /// <para>The name of this method is pre-defined and must not be changed.</para>
        /// </remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //
            // Each "Use...()" method registers a middleware in the pipeline.
            //
            // Calls to "Map...()" methods create branches in the middleware pipeline; see:
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#branch-the-middleware-pipeline
            //
            // For a full overview, see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/
            //
            // IMPORTANT: The order of the "Use...()" method calls is important as it defines
            //   the order of the middleware components in the pipeline!
            //

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Enable routing feature; required for defining endpoints below.
            // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#routing-basics
            app.UseRouting();

            app.UseAuthorization();

            // Define endpoints (invokable actions). Requires call to "UseRouting()" above.
            // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
