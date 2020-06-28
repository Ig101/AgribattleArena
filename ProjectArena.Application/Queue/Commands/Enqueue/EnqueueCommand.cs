using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Application.Queue.Commands.Dequeue;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.QueueService;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.ErrorHandling;
using ProjectArena.Infrastructure.Models.Game;
using ProjectArena.Infrastructure.Models.Queue;
using ProjectArena.Infrastructure.Models.User;

namespace ProjectArena.Application.Queue.Commands.Enqueue
{
    public class EnqueueCommand : IRequest
    {
        public string UserId { get; set; }

        public GameMode Mode { get; set; }

        internal class Handler : IRequestHandler<EnqueueCommand>
        {
            private readonly IQueueService _queueService;
            private readonly IBattleService _battleService;
            private readonly GameContext _gameContext;

            public Handler (
                IQueueService queueService,
                IBattleService battleService,
                GameContext gameContext)
            {
                _queueService = queueService;
                _battleService = battleService;
                _gameContext = gameContext;
            }

            public async Task<Unit> Handle(EnqueueCommand request, CancellationToken cancellationToken)
            {
                var roster = await _gameContext.Rosters.GetOneAsync(x => x.UserId == request.UserId);
                var characters = await _gameContext.Characters.GetAsync(x => x.RosterId == roster.Id && !x.Deleted);
                if (characters.Count() != 3)
                {
                    throw new CannotPerformOperationException("Wrong amount of characters");
                }

                if (_battleService.IsUserInBattle(request.UserId))
                {
                    throw new CannotPerformOperationException("User is in battle");
                }

                var result = _queueService.Enqueue(new UserToEnqueueDto()
                {
                    UserId = request.UserId,
                    Mode = request.Mode
                });
                if (!result)
                {
                    throw new CannotPerformOperationException("User is already in queue");
                }

                return Unit.Value;
            }
        }
    }
}