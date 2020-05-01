using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.QueueService;

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
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _logger.LogDebug($"{Context.UserIdentifier} disconnected from hub");
        _queueService.Dequeue(Context.UserIdentifier);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendSynchronizationErrorAsync(string userId)
    {
        await this.Clients.User(userId).SendAsync("BattleSynchronizationError");
    }

    public async Task OrderAttackAsync(int actorId, int targetX, int targetY)
    {
        var userId = Context.UserIdentifier;
        var scene = _battleService.GetUserScene(userId);
        bool result = false;
        if (scene.GetPlayerActors(userId).Contains(actorId))
        {
            result = scene.ActorAttack(actorId, targetX, targetY);
        }

        if (!result)
        {
            await SendSynchronizationErrorAsync(userId);
        }
    }

    public async Task OrderMoveAsync(int actorId, int targetX, int targetY)
    {
        var userId = Context.UserIdentifier;
        var scene = _battleService.GetUserScene(userId);
        bool result = false;
        if (scene.GetPlayerActors(userId).Contains(actorId))
        {
            result = scene.ActorMove(actorId, targetX, targetY);
        }

        if (!result)
        {
            await SendSynchronizationErrorAsync(userId);
        }
    }

    public async Task OrderCastAsync(int actorId, int skillId, int targetX, int targetY)
    {
        var userId = Context.UserIdentifier;
        var scene = _battleService.GetUserScene(userId);
        bool result = false;
        if (scene.GetPlayerActors(userId).Contains(actorId))
        {
            result = scene.ActorCast(actorId, skillId, targetX, targetY);
        }

        if (!result)
        {
            await SendSynchronizationErrorAsync(userId);
        }
    }

    public async Task OrderWaitAsync(int actorId)
    {
        var userId = Context.UserIdentifier;
        var scene = _battleService.GetUserScene(userId);
        bool result = false;
        if (scene.GetPlayerActors(userId).Contains(actorId))
        {
            result = scene.ActorWait(actorId);
        }

        if (!result)
        {
            await SendSynchronizationErrorAsync(userId);
        }
    }
  }
}