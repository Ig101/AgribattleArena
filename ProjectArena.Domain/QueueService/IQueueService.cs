using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Infrastructure.Models.Queue;

namespace ProjectArena.Domain.QueueService
{
    public interface IQueueService
    {
        void AddBot(BotDefinition definition);

        void RemoveBot(string id);

        void QueueProcessing(double time);

        bool Enqueue(UserToEnqueueDto user);

        void Dequeue(string userId);

        UserInQueueDto IsUserInQueue(string userId);
    }
}