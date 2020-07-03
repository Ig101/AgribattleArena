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
        private const int RandomModifier = 10000;

        private readonly IDictionary<GameMode, SceneModeQueue> _queues;

        private readonly Random _botsRandom;
        private readonly ICollection<BotDefinition> _bots;

        private readonly IBattleService _battleService;
        private readonly object _locker = new object();

        public QueueService(
            IBattleService battleService)
        {
            _battleService = battleService;
            _botsRandom = new Random();
            _bots = new HashSet<BotDefinition>();
            _queues = BattleHelper.GetNewModeQueue();
        }

        public void AddBot(BotDefinition definition)
        {
            _bots.Add(definition);
        }

        public void RemoveBot(string id)
        {
            var bot = _bots.FirstOrDefault(x => x.BotId == id);
            _bots.Remove(bot);
        }

        public void QueueProcessing(double time)
        {
            foreach (var queue in _queues.Values)
            {
                var complectingUsers = new List<List<UserInQueue>>();
                var complectedUsers = new List<List<UserInQueue>>();
                foreach (var user in queue.Queue)
                {
                    var complect = queue.Mode.BotsOnly ?
                        null :
                        complectingUsers.FirstOrDefault(x => !x.Any(complectingUser => complectingUser.UserId == user.UserId));
                    if (complect != null)
                    {
                        complect.Add(user);
                        if (complect.Count >= queue.Mode.MaxPlayers)
                        {
                            complectedUsers.Add(complect);
                            complectingUsers.RemoveAt(0);
                        }
                    }
                    else
                    {
                        complectingUsers.Add(new List<UserInQueue>() { user });
                    }

                    user.Time += time;
                }

                if (_bots.Count > 0 && queue.Mode.TimeTillBot.HasValue)
                {
                    foreach (var complectingUsersItem in complectingUsers)
                    {
                        if (complectingUsersItem.Average(x => x.Time) >= queue.Mode.TimeTillBot.Value)
                        {
                            while (complectingUsersItem.Count < queue.Mode.MaxPlayers)
                            {
                                var chosenBot = _botsRandom.Next(_bots.Count * RandomModifier) % _bots.Count;
                                complectingUsersItem.Add(new UserInQueue()
                                {
                                    UserId = _bots.Skip(chosenBot).First().BotId
                                });
                            }

                            complectedUsers.Add(complectingUsersItem);
                        }
                    }
                }

                var allComplectedActors = complectedUsers.SelectMany(actor => actor).ToList();
                queue.Queue.RemoveWhere(x => allComplectedActors.Contains(x));
                foreach (var complect in complectedUsers)
                {
                    Task.Run(async () => await _battleService.StartNewBattleAsync(queue.Mode, complect));
                }
            }
        }

        private void DequeueInternal(string userId, GameMode? modeToIgnore = null)
        {
            foreach (var queue in _queues)
            {
                if (queue.Key == modeToIgnore)
                {
                    continue;
                }

                queue.Value.Queue.RemoveWhere(x => x.UserId == userId);
            }
        }

        public bool Enqueue(UserToEnqueueDto user)
        {
            var targetQueue = _queues[user.Mode];
            if (!targetQueue.Mode.AllowMultiEnqueue && targetQueue.Queue.Any(x => x.UserId == user.UserId))
            {
                return false;
            }

            lock (_locker)
            {
                DequeueInternal(user.UserId, user.Mode);
                targetQueue.Queue.Add(new UserInQueue()
                {
                    UserId = user.UserId
                });
                return true;
            }
        }

        public void Dequeue(string userId)
        {
            lock (_locker)
            {
                DequeueInternal(userId);
            }
        }

        public UserInQueueDto IsUserInQueue(string userId)
        {
            lock (_locker)
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
                    .FirstOrDefault();
            }
        }
    }
}