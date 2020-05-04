using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.QueueService;

namespace ProjectArena.Domain.ArenaHub
{
    public class ArenaHostedService : IHostedService, IDisposable
    {
        private readonly IQueueService _queueService;
        private readonly IBattleService _battleService;
        private readonly ILogger<ArenaHostedService> _logger;
        private DateTime _timeStamp;
        private Timer _timer;

        public ArenaHostedService(
            IQueueService queueService,
            IBattleService battleService,
            ILogger<ArenaHostedService> logger)
        {
            _queueService = queueService;
            _battleService = battleService;
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(BackgroundProcessing, 5, TimeSpan.Zero, TimeSpan.FromSeconds(5));
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
            var currentTime = DateTime.Now;
            var passed = (currentTime - _timeStamp).TotalSeconds;
            _timeStamp = currentTime;
            _queueService.QueueProcessing(passed);
            _battleService.EngineTimeProcessing(passed);
        }
    }
}