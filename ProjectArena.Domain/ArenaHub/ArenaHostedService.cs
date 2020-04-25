using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.QueueService;

namespace ProjectArena.Domain.ArenaHub
{
    public class ArenaHostedService : BackgroundService
    {
        private readonly IQueueService _queueService;
        private readonly IBattleService _battleService;

        private DateTime _timeStamp;

        public ArenaHostedService(
            IQueueService queueService,
            IBattleService battleService)
        {
            _queueService = queueService;
            _battleService = battleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var currentTime = DateTime.Now;
            var passed = (currentTime - _timeStamp).TotalSeconds;
            _timeStamp = currentTime;
            await _queueService.QueueProcessingAsync(passed);
            _battleService.EngineTimeProcessing(passed);
        }
    }
}