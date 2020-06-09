using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.BattleService;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Battle.Queries.GetFullSynchronizationInfo
{
    public class GetFullSynchronizationInfoQuery : IRequest<SynchronizerDto>
    {
        public string UserId { get; set; }

        internal class Handler : IRequestHandler<GetFullSynchronizationInfoQuery, SynchronizerDto>
        {
            private readonly IBattleService _battleService;

            public Handler(IBattleService battleService)
            {
                _battleService = battleService;
            }

            public Task<SynchronizerDto> Handle(GetFullSynchronizationInfoQuery request, CancellationToken cancellationToken)
            {
                return Task.Run(() =>
                {
                    var result = _battleService.GetUserSynchronizationInfo(request.UserId);
                    if (result == null)
                    {
                        throw new CannotPerformOperationException("User is not in battle");
                    }

                    return result;
                });
            }
        }
    }
}