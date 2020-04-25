using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.BattleService.Helpers;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Queue;

namespace ProjectArena.Domain.QueueService
{
    public class QueueService : IQueueService
    {
        private readonly IDictionary<GameMode, SceneModeQueue> _queues;
        private readonly IBattleService _battleService;

        public QueueService(
            IBattleService battleService)
        {
            _battleService = battleService;
            _queues = BattleHelper.GetNewModeQueue();
        }

        public async Task QueueProcessingAsync(double time)
        {
            foreach (var queue in _queues.Values)
            {
                var complectingActors = new List<List<UserInQueue>>();
                var complectedActors = new List<List<UserInQueue>>();
                foreach (var user in queue.Queue)
                {
                    bool added = false;
                    if (complectedActors.Count > 0)
                    {
                        added = true;
                        var complect = complectingActors[0];
                        complect.Add(user);
                        if (complect.Count >= queue.Mode.MaxPlayers)
                        {
                            complectedActors.Add(complect);
                            complectingActors.RemoveAt(0);
                        }
                    }

                    // TODO When there will be logic
                    /*for (int j = 0; j < complectingActors.Count; j++)
                    {
                        var complect = complectingActors[j];
                        complect.Add(user);
                        if (complect.Count >= queue.Mode.MaxPlayers)
                        {
                            complectedActors.Add(complect);
                            complectingActors.RemoveAt(j);
                        }

                        added = true;
                        break;
                    }*/

                    if (!added)
                    {
                        complectingActors.Add(new List<UserInQueue>() { user });
                    }

                    user.Time += time;
                }

                queue.Queue = queue.Queue.Except(complectedActors.SelectMany(x => x).ToList()).ToList();
                foreach (var complect in complectedActors)
                {
                    await _battleService.StartNewBattleAsync(queue.Mode, complect);
                }
            }
        }

        public void Enqueue(UserToEnqueueDto user)
        {
            var targetQueue = _queues[user.Mode];
            targetQueue.Queue.Add(new UserInQueue()
            {
                UserId = user.UserId
            });
        }

        public void Dequeue(string userId)
        {
            foreach (var queue in _queues.Values)
            {
                UserInQueue user;
                if ((user = queue.Queue.FirstOrDefault(x => x.UserId == userId)) != null)
                {
                    queue.Queue.Remove(user);
                }
            }
        }

        public IEnumerable<UserInQueueDto> IsUserInQueue(string userId)
        {
            return _queues
                .Select(
                    x => x.Value.Queue.FirstOrDefault(member => member.UserId == userId)?.Time == null ? (UserInQueueDto)null : new UserInQueueDto()
                    {
                        Mode = x.Key,
                        Time = (int)x.Value.Queue.FirstOrDefault(member => member.UserId == userId).Time
                    })
                .Where(
                    x => x != null)
                .ToList();
        }
    }
}