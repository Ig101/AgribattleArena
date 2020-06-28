using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.QueueService;
using ProjectArena.Infrastructure.Models.Game;

namespace ProjectArena.Domain.ArenaHub
{
    public class ArenaHostedService : IHostedService, IDisposable
    {
        private readonly IQueueService _queueService;
        private readonly IBattleService _battleService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ArenaHostedService> _logger;
        private DateTime _timeStamp;
        private Timer _timer;
        private DateTime? _lastUpdateDate;

        public ArenaHostedService(
            IQueueService queueService,
            IBattleService battleService,
            IServiceProvider serviceProvider,
            ILogger<ArenaHostedService> logger)
        {
            _queueService = queueService;
            _battleService = battleService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(BackgroundProcessing, 2, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            _timeStamp = DateTime.Now.ToUniversalTime();
            _logger.LogInformation("Arena hosted service is running.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Arena hosted service is stopped.");
            return Task.CompletedTask;
        }

        private void BackgroundProcessing(object timer)
        {
            var currentTime = DateTime.Now.ToUniversalTime();

            if (_lastUpdateDate == null)
            {
                var gameContext = _serviceProvider.GetRequiredService<GameContext>();
                var gameInfo = gameContext.GameInfo.GetOneAsync(x => x.Actual).Result;
                if (gameInfo == null)
                {
                    gameInfo = new Game.Entities.GameInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Actual = true,
                        LastUpdateDate = currentTime.Date
                    };
                    gameContext.GameInfo.InsertOneAtomicallyAsync(gameInfo).Wait();
                }

                _lastUpdateDate = gameInfo.LastUpdateDate;
            }

            var passed = (currentTime - _timeStamp).TotalSeconds;
            _queueService.QueueProcessing(passed);
            _battleService.EngineTimeProcessing(passed);
            _timeStamp = currentTime;

            if (currentTime.Date != _lastUpdateDate.Value.Date)
            {
                _lastUpdateDate = currentTime.Date;
                Task.Run(async () => await DailyUpdateAsync(_lastUpdateDate.Value));
            }
        }

        private async Task DailyUpdateAsync(DateTime date)
        {
            _logger.LogInformation("Started daily update");
            var random = new Random();
            var gameContext = _serviceProvider.GetRequiredService<GameContext>();
            gameContext.Rosters.Update(
                x => true,
                Builders<Roster>.Update
                .Set(x => x.BoughtPatrons, new int[0])
                .Set(x => x.Seed, random.Next()));
            gameContext.GameInfo.Update(
                x => x.Actual,
                Builders<GameInfo>.Update.Set(x => x.LastUpdateDate, date));
            await gameContext.ApplyChangesAsync();
            var arenaHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub>>();
            var info = new DailyChangedInfoDto()
            {
                Tavern = new[]
                {
                    new CharacterForSaleDto() { Id = 1 },
                    new CharacterForSaleDto() { Id = 2 },
                    new CharacterForSaleDto() { Id = 3 },
                    new CharacterForSaleDto() { Id = 4 },
                    new CharacterForSaleDto() { Id = 5 },
                    new CharacterForSaleDto() { Id = 6 }
                }
            };
            await arenaHub.Clients.All.SendAsync("DailyUpdate", info);
            _logger.LogInformation("Finished daily update");
        }
    }
}