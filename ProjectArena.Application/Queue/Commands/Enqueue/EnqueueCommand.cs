using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Application.Queue.Commands.Dequeue;
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
            private readonly IdentityUserManager _userManager;
            private readonly IQueueService _queueService;
            private readonly GameContext _gameContext;

            public Handler (
                IdentityUserManager userManager,
                IQueueService queueService,
                GameContext gameContext)
            {
                _userManager = userManager;
                _queueService = queueService;
                _gameContext = gameContext;
            }

            public async Task<Unit> Handle(EnqueueCommand request, CancellationToken cancellationToken)
            {
                var roster = await _gameContext.Rosters.GetOneAsync(x => x.UserId == request.UserId);
                var characters = await _gameContext.Characters.GetAsync(x => x.RosterUserId == request.UserId && !x.Deleted);
                if (characters.Count() != 6)
                {
                    throw new HttpException()
                    {
                        Error = "Wrong amount of characters",
                        StatusCode = 400
                    };
                }

                var result = _queueService.Enqueue(new UserToEnqueueDto()
                {
                    UserId = request.UserId,
                    Mode = request.Mode
                });
                if (!result)
                {
                    throw new HttpException()
                    {
                        Error = "User is already in queue",
                        StatusCode = 400
                    };
                }

                return Unit.Value;
            }
        }
    }
}