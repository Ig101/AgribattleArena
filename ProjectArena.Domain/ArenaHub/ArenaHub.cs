using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.QueueService;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Infrastructure.Models.Game;

namespace ProjectArena.Domain.ArenaHub
{
    [Authorize]
    public class ArenaHub : Hub
    {
        private readonly ILogger<ArenaHub> _logger;
        private readonly IBattleService _battleService;
        private readonly IQueueService _queueService;

        public ArenaHub(
            IBattleService battleService,
            IQueueService queueService,
            ILogger<ArenaHub> logger)
        {
            _battleService = battleService;
            _queueService = queueService;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogDebug($"{Context.UserIdentifier} connected to hub");
            if (Context.User.IsInRole("bot"))
            {
                _queueService.AddBot(new BotDefinition()
                {
                    BotId = Context.UserIdentifier
                });
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogDebug($"{Context.UserIdentifier} disconnected from hub");
            if (Context.User.IsInRole("bot"))
            {
                _queueService.RemoveBot(Context.UserIdentifier);
            }
            else
            {
                _queueService.Dequeue(Context.UserIdentifier);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendSynchronizationErrorAsync(string userId)
        {
            await this.Clients.User(userId).SendAsync("BattleSynchronizationError");
        }

        public void OrderAttack(Guid sceneId, string playerId, int targetX, int targetY)
        {
            var userId = Context.UserIdentifier;
            var scene = _battleService.GetUserScene(userId, sceneId);
            if (scene.ShortPlayers.Any(x => x.Id == playerId))
            {
                scene.ActorAttack(playerId, targetX, targetY);
            }
        }

        public void OrderMove(Guid sceneId, string playerId, int targetX, int targetY)
        {
            var userId = Context.UserIdentifier;
            var scene = _battleService.GetUserScene(userId, sceneId);
            if (scene.ShortPlayers.Any(x => x.Id == playerId))
            {
                scene.ActorMove(playerId, targetX, targetY);
            }
        }

        public void OrderCast(Guid sceneId, string playerId, int skillId, int targetX, int targetY)
        {
            var userId = Context.UserIdentifier;
            var scene = _battleService.GetUserScene(userId, sceneId);
            if (scene.ShortPlayers.Any(x => x.Id == playerId))
            {
                scene.ActorCast(playerId, skillId, targetX, targetY);
            }
        }

        public void OrderOrder(Guid sceneId, int actorId, int skillId, int targetX, int targetY)
        {
            var userId = Context.UserIdentifier;
            var scene = _battleService.GetUserScene(userId, sceneId);
            if (scene.GetUserActors(userId).Contains(actorId))
            {
                scene.ActorOrder(actorId, skillId, targetX, targetY);
            }
        }
    }
}