using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Nomad
{
    public class Startup
    {
        private static readonly string ClientId = Environment.GetEnvironmentVariable("OPENID_CLIENTID");
        private static readonly string ClientSecret = Environment.GetEnvironmentVariable("OPENID_CLIENTSECRET");
        private static readonly string Authority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_");
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Nomad", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            if (!string.IsNullOrEmpty(ClientId) || !string.IsNullOrEmpty(ClientSecret) || !string.IsNullOrEmpty(Authority))
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = "cookies",
                    AutomaticAuthenticate = true,
                    LoginPath = new PathString("/signin"),
                    LogoutPath = new PathString("/signout")
                });

                app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    AuthenticationScheme = OpenIdConnectDefaults.AuthenticationScheme,
                    SignInScheme = "cookies",
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    Authority = Authority,
                    ResponseType = "code id_token",
                    GetClaimsFromUserInfoEndpoint = true,
                    SaveTokens = true,
                    UseTokenLifetime = true
                });
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Dashboard}/{action=Index}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nomad");
            });
        }
    }
}
