using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.BattleService;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Battle.Queries.GetFullSynchronizationInfo
{
    public class LeaveSceneCommand : IRequest
    {
        public string UserId { get; set; }

        public Guid SceneId { get; set; }

        internal class Handler : IRequestHandler<LeaveSceneCommand>
        {
            private readonly IBattleService _battleService;

            public Handler(IBattleService battleService)
            {
                _battleService = battleService;
            }

            public Task<Unit> Handle(LeaveSceneCommand request, CancellationToken cancellationToken)
            {
                return Task.Run(() =>
                {
                    var result = _battleService.LeaveScene(request.UserId, request.SceneId);
                    if (!result)
                    {
                        throw new CannotPerformOperationException("User is not in battle");
                    }

                    return Unit.Value;
                });
            }
        }
    }
}