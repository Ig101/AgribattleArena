using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.QueueService;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Application.Queue.Commands.Dequeue
{
    public class DequeueCommand : IRequest
    {
        public string UserId { get; set; }

        internal class Handler : IRequestHandler<DequeueCommand>
        {
            private readonly IdentityUserManager _userManager;
            private readonly IQueueService _queueService;

            public Handler (
                IdentityUserManager userManager,
                IQueueService queueService)
            {
                _userManager = userManager;
                _queueService = queueService;
            }

            public Task<Unit> Handle(DequeueCommand request, CancellationToken cancellationToken)
            {
                _queueService.Dequeue(request.UserId);
                return Unit.Task;
            }
        }
    }
}