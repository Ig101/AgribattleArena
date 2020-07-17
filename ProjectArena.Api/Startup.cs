using System;
using System.IO;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProjectArena.Api.Filters;
using ProjectArena.Application;
using ProjectArena.Domain;
using ProjectArena.Domain.ArenaHub;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Email;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.QueueService;
using ProjectArena.Domain.Registry;
using ProjectArena.Infrastructure;
using ProjectArena.Infrastructure.Mongo;
using Serilog.Events;

namespace ProjectArena.Api
{
  public class Startup
    {
        public static LogEventLevel LogEventLevel { get; set; }

        private readonly Func<HttpContext, Func<Task>, Task> _routingMiddleware =
            async (context, next) =>
            {
                if (IsFrontendRoute(context))
                {
                    context.Request.Path = new PathString("/index.html");
                }

                await next();
            };

        public Startup(IWebHostEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            LogEventLevel = (LogEventLevel)Enum.Parse(LogEventLevel.GetType(), Configuration["Logging:Level"]);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(ValidationFilter));
                    options.Filters.Add(typeof(ExceptionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssembly(ApplicationRegistry.GetAssembly());
                });
            services.Configure<ServerSettings>(
                Configuration.GetSection("Server"));
            services.Configure<MongoConnectionSettings>(
                Configuration.GetSection("MongoConnection"));
            services.Configure<MongoContextSettings<RegistryContext>>(
                Configuration.GetSection("MongoConnection:Registry"));
            services.Configure<MongoContextSettings<GameContext>>(
                Configuration.GetSection("MongoConnection:Game"));
            services.Configure<EmailSenderSettings>(
                Configuration.GetSection("SmtpServer"));
            services.Configure<DefaultUsersSettings>(
                Configuration.GetSection("DefaultUsers"));
            services.RegisterDomainLayer(Configuration["MongoConnection:ConnectionString"]);
            services.RegisterApplicationLayer();
        }

        public static bool IsFrontendRoute(HttpContext context)
        {
            var path = context.Request.Path;
            return path.HasValue &&
                !Path.HasExtension(path) &&
                !path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase) &&
                !path.Value.StartsWith("/hub", StringComparison.OrdinalIgnoreCase);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.Use(_routingMiddleware);
                app.UseDefaultFiles();
                app.UseStaticFiles(new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                });
            }

            app.UseDomainLayer();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ArenaHub>("/hub");
            });
        }
    }
}
