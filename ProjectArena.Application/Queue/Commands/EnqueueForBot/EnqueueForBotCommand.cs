using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Application.Queue.Commands.Enqueue;
using ProjectArena.Domain.QueueService;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.ErrorHandling;
using ProjectArena.Infrastructure.Models.Queue;

namespace ProjectArena.Application.Queue.Commands.EnqueueForBot
{
    public class EnqueueForBotCommand : IRequest
    {
        public string UserId { get; set; }

        internal class Handler : IRequestHandler<EnqueueForBotCommand>
        {
            private readonly IQueueService _queueService;

            public Handler (
                IQueueService queueService)
            {
                _queueService = queueService;
            }

            public Task<Unit> Handle(EnqueueForBotCommand request, CancellationToken cancellationToken)
            {
                var result = _queueService.Enqueue(new UserToEnqueueDto()
                {
                    UserId = request.UserId,
                    Mode = GameMode.BotLearning
                });
                if (!result)
                {
                    throw new CannotPerformOperationException("Bot is already in queue");
                }

                return Unit.Task;
            }
        }
    }
}