using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectArena.Infrastructure.Models.Queue;

namespace ProjectArena.Domain.QueueService
{
    public interface IQueueService
    {
        Task QueueProcessingAsync(double time);

        bool Enqueue(UserToEnqueueDto user);

        void Dequeue(string userId);

        UserInQueueDto IsUserInQueue(string userId);
    }
}