using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Services.AllocationLogProviders;

namespace Nomad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var allocationLogsProviderClientBindings = new List<AllocationLogsProviderClientBinding>();
            Configuration.GetSection(nameof(AllocationLogsProviderClientBinding)).Bind(allocationLogsProviderClientBindings);

            var allocationProviderFactory = new DefaultAllocationLogProviderFactoryLoader(allocationLogsProviderClientBindings)
                .GetAllocationLogProviderFactoryAsync().Result;

            services.AddSingleton(allocationProviderFactory);
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Dashboard", action = "Index" });
            });
        }
    }
}
