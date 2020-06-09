using System.Reflection;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectArena.Domain.ArenaHub;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Email;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Domain.Identity.EntityConfiguration;
using ProjectArena.Domain.Registry;
using ProjectArena.Infrastructure.Mongo;

namespace ProjectArena.Domain
{
    public static class DomainRegistry
    {
        public static IApplicationBuilder UseDomainLayer(this IApplicationBuilder app)
        {
            var usersCollection = app.ApplicationServices.GetRequiredService<IMongoCollection<User>>();
            new UserConfiguration().ConfigureAsync(usersCollection).Wait();
            var rolesCollection = app.ApplicationServices.GetRequiredService<IMongoCollection<Role>>();
            new RoleConfiguration().ConfigureAsync(rolesCollection).Wait();

            app.ApplicationServices.GetRequiredService<IMongoConnection>();

            var registry = app.ApplicationServices.GetRequiredService<RegistryContext>();
            registry.LoadMigrations();

            var battleService = app.ApplicationServices.GetRequiredService<IBattleService>();
            battleService.Init();

            return app;
        }

        public static IServiceCollection RegisterDomainLayer(this IServiceCollection services, string identityUrl)
        {
            services.AddIdentityMongoDbProvider<User, Role>(
                identityOptions =>
                {
                    identityOptions.Password.RequiredLength = 8;
                    identityOptions.Password.RequireLowercase = true;
                    identityOptions.Password.RequireUppercase = true;
                    identityOptions.Password.RequireNonAlphanumeric = false;
                    identityOptions.Password.RequireDigit = true;
                    identityOptions.User.RequireUniqueEmail = true;
                    identityOptions.SignIn.RequireConfirmedEmail = true;
                }, mongoIdentityOptions =>
                {
                    mongoIdentityOptions.ConnectionString = identityUrl;
                })
                .AddUserManager<IdentityUserManager>()
                .AddRoleManager<IdentityRoleManager>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "Authorization";
                options.Cookie.HttpOnly = true;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
                options.LoginPath = "/api/Account/Login";
                options.AccessDeniedPath = "/api/Account/AccessDenied";
            });
            var domainAssembly = Assembly.GetExecutingAssembly();
            services.AddSingleton<IMongoConnection, MongoConnection>(
                provider => new MongoConnection(
                    domainAssembly,
                    provider.GetRequiredService<IOptions<MongoConnectionSettings>>(),
                    provider));
            services.AddTransient<RegistryContext>();
            services.AddTransient<GameContext>();
            services.AddTransient<EmailSender>();
            services.AddSignalR();
            services.AddSingleton<BattleService.IBattleService, BattleService.BattleService>();
            services.AddSingleton<QueueService.IQueueService, QueueService.QueueService>();
            services.AddHostedService<ArenaHostedService>();
            return services;
        }
    }
}