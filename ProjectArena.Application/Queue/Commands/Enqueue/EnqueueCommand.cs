using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Application.Queue.Commands.Dequeue;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.QueueService;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.ErrorHandling;

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

            public Handler (
                IdentityUserManager userManager,
                IQueueService queueService)
            {
                _userManager = userManager;
                _queueService = queueService;
            }

            public Task<Unit> Handle(EnqueueCommand request, CancellationToken cancellationToken)
            {
                var result = _queueService.Enqueue(new Infrastructure.Models.Queue.UserToEnqueueDto()
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

                return Unit.Task;
            }
        }
    }
}