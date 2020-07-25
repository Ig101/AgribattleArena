using System;
using System.Reflection;
using System.Threading.Tasks;
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
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Domain.Identity.EntityConfiguration;
using ProjectArena.Domain.Registry;
using ProjectArena.Infrastructure.Mongo;

namespace ProjectArena.Domain
{
  public static class DomainRegistry
    {
        private static IApplicationBuilder UseDomainIdentity(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var provider = scope.ServiceProvider;

            var usersCollection = provider.GetRequiredService<IMongoCollection<User>>();
            new UserConfiguration().ConfigureAsync(usersCollection).Wait();

            var rolesCollection = provider.GetRequiredService<IMongoCollection<Role>>();
            new RoleConfiguration().ConfigureAsync(rolesCollection).Wait();

            var roleManager = provider.GetRequiredService<IdentityRoleManager>();
            if (!roleManager.RoleExistsAsync("admin").Result)
            {
                roleManager.CreateAsync(new Role("admin")).Wait();
            }

            if (!roleManager.RoleExistsAsync("bot").Result)
            {
                roleManager.CreateAsync(new Role("bot")).Wait();
            }

            var userSettings = provider.GetRequiredService<IOptions<DefaultUsersSettings>>().Value;
            var userManager = provider.GetRequiredService<IdentityUserManager>();

            var rootEmail = $"{userSettings.Root.Login}@{userSettings.Root.Login}";
            var result = userManager.FindByEmailAsync(rootEmail).Result;
            if (userManager.FindByEmailAsync(rootEmail).Result == null)
            {
                var user = new User()
                {
                    UserName = Guid.NewGuid().ToString(),
                    Email = rootEmail,
                    ViewName = userSettings.Root.Login,
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, userSettings.Root.Password).Wait();
                userManager.AddToRoleAsync(user, "admin").Wait();
            }

            var gameContext = provider.GetRequiredService<GameContext>();
            if (userSettings.Bots != null)
            {
                foreach (var bot in userSettings.Bots)
                {
                    var botEmail = $"{bot.Login}@{bot.Login}";
                    if (userManager.FindByEmailAsync(botEmail).Result == null)
                    {
                        var botId = Guid.NewGuid().ToString();
                        var user = new User()
                        {
                            Id = botId,
                            UserName = Guid.NewGuid().ToString(),
                            Email = botEmail,
                            ViewName = bot.Login,
                            EmailConfirmed = true
                        };
                        userManager.CreateAsync(user, bot.Password).Wait();
                        userManager.AddToRoleAsync(user, "bot").Wait();
                        gameContext.UseTankMDDRDDBotRoster(botId);
                        gameContext.UseTankRDDRDDBotRoster(botId);
                        gameContext.UseThreeMagesBotRoster(botId);
                        gameContext.UseThreeMDDBotRoster(botId);
                        gameContext.UseThreeSummonersBotRoster(botId);
                        gameContext.UseThreeTanksBotRoster(botId);
                    }
                }
            }

            gameContext.ApplyChangesAsync().Wait();

            return app;
        }

        public static IApplicationBuilder UseDomainLayer(this IApplicationBuilder app)
        {
            app.UseDomainIdentity();

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
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
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