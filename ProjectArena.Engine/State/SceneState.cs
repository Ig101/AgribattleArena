using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.Helpers;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Battle;
using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine.State
{
    public class SceneState
    {
        public string Id { get; set; }

        public double TimeLine { get; set; }

        public int IdCounterPosition { get; set; }

        public StartTurnInfoDto TurnInfo { get; set; }

        public IList<ActorSynchronizationDto> Actors { get; set; }

        public IEnumerable<PlayerState> Players { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Biom Biom { get; set; }

        public SceneState Clone()
        {
            return new SceneState
            {
                Id = Id,
                TimeLine = TimeLine,
                IdCounterPosition = IdCounterPosition,
                TurnInfo = new StartTurnInfoDto
                {
                    TempActor = TurnInfo.TempActor,
                    Time = TurnInfo.Time
                },
                Width = Width,
                Height = Height,
                Biom = Biom,
                Players = Players.Select(p => new PlayerState
                {
                    Id = p.Id,
                    Reward = p.Reward,
                    BattlePlayerStatus = p.BattlePlayerStatus
                }).ToList(),
                Actors = Actors.Select(a => ActorsHelper.CloneActor(a)).ToList()
            };
        }

        public (ActorSynchronizationDto actor, ActorSynchronizationDto parent) FindActorWithParent(ActorReferenceDto reference)
        {
            var suitableActors = Actors.Where(a => a.Reference.X == reference.X && a.Reference.Y == reference.Y);
            var resultActor = suitableActors.FirstOrDefault(a => a.Reference.Id == reference.Id);
            if (resultActor != null)
            {
                return (resultActor, null);
            }

            foreach (var actor in suitableActors)
            {
                var result = ActorsHelper.FindActorWithParent(reference, actor);
                if (result != (null, null))
                {
                    return result;
                }
            }

            return (null, null);
        }

        public IEnumerable<ActorSynchronizationDto> GetAllPlayerActors(string playerId, ActorSynchronizationDto root = null)
        {
            var actorsList = new List<ActorSynchronizationDto>();

            var initialActorsArray = root.Actors ?? Actors;
            foreach (var actor in initialActorsArray)
            {
                if (actor.OwnerId == playerId)
                {
                    actorsList.Add(actor);
                }

                actorsList.AddRange(GetAllPlayerActors(playerId, actor));
            }

            return actorsList;
        }

        public void ProcessAllActors(Action<SceneState, ActorSynchronizationDto> action, ActorSynchronizationDto root = null)
        {
            var initialActorsArray = root.Actors ?? Actors;
            foreach (var actor in initialActorsArray)
            {
                action(this, actor);
                ProcessAllActors(action, actor);
            }
        }

        public (ActorSynchronizationDto actor, ActionSynchronizationDto action) FindNextAutomaticAction()
        {
            foreach (var actor in Actors)
            {
                var action = actor.Actions.FirstOrDefault(a => a.IsAutomatic && a.RemainedTime <= 0);
                if (action != null)
                {
                    return (actor, action);
                }

                foreach (var childActor in actor.Actors)
                {
                    var childAction = childActor.Actions.FirstOrDefault(a => a.IsAutomatic && a.RemainedTime <= 0);
                    if (childAction != null)
                    {
                        return (childActor, childAction);
                    }
                }
            }

            return (null, null);
        }

        public void MergeSynchronizer(SynchronizerDto synchronizer)
        {
            IdCounterPosition = synchronizer.IdCounterPosition;
            foreach (var reference in synchronizer.RemovedActors)
            {
                var (actor, parent) = FindActorWithParent(reference);
                if (actor != null)
                {
                    var actorsArray = parent?.Actors ?? Actors;
                    actorsArray.Remove(actor);
                }
            }

            ActorsHelper.MergeActors(synchronizer.Actors, Actors);
        }
    }
}