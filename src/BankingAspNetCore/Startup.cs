using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BankingCore.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using BankingCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace BankingAspNetCore
{
    public class Startup
    {
        readonly IHostingEnvironment HostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BankingContext>(builder =>
            {
                string useSqLite = Configuration["Data:useSqLite"];
                if (useSqLite != "true")
                {
                    var connStr = Configuration["Data:SqlServerConnectionString"];
                    builder.UseSqlServer(connStr);
                }
                else
                {
                    var connStr = "Data Source=" +
                                  Path.Combine(HostingEnvironment.ContentRootPath, "BankingData.sqlite");
                    builder.UseSqlite(connStr);
                }
            });

            //services.AddSingleton<IConfigurationRoot>(Configuration);
            //services.AddSingleton<IConfiguration>(Configuration);

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IUserTransactionRepository, UserTransactionRepository>();

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies").RequireAuthenticatedUser().Build();
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, BankingContext dbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                LoginPath = new PathString("/Account/Login"),
                AutomaticAuthenticate = true,
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            dbContext.Database.EnsureCreated();

        }
    }
}
