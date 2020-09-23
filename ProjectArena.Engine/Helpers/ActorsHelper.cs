using System.Collections.Generic;
using System.Linq;
using ProjectArena.Infrastructure.Models.Battle;

namespace ProjectArena.Engine.Helpers
{
    public class ActorsHelper
    {
        public static ActorSynchronizationDto CloneActor(ActorSynchronizationDto actor)
        {
            return new ActorSynchronizationDto
            {
                Reference = actor.Reference,
                Name = actor.Name,
                Char = actor.Char,
                Color = actor.Color,
                Left = actor.Left,
                OwnerId = actor.OwnerId,
                Tags = actor.Tags,
                ParentId = actor.ParentId,
                Durability = actor.Durability,
                MaxDurability = actor.MaxDurability,
                Initiative = actor.Initiative,
                InitiativePosition = actor.InitiativePosition,
                Height = actor.Height,
                Volume = actor.Volume,
                FreeVolume = actor.FreeVolume,
                PreparationReactions = actor.PreparationReactions,
                ActiveReactions = actor.ActiveReactions,
                ClearReactions = actor.ClearReactions,
                Actions = actor.Actions,
                Buffs = actor.Buffs,
                Actors = actor.Actors.Select(a => CloneActor(a)).ToList()
            };
        }

        public static (ActorSynchronizationDto actor, ActorSynchronizationDto parent) FindActorWithParent(ActorReferenceDto reference, ActorSynchronizationDto parent)
        {
            foreach (var actor in parent.Actors)
            {
                var result = actor.Reference.Id == reference.Id ? (actor, parent) : FindActorWithParent(reference, actor);
                if (result != (null, null))
                {
                    return result;
                }
            }

            return (null, null);
        }

        public static void MergeActors(IEnumerable<ActorSynchronizationDto> sourceArray, IList<ActorSynchronizationDto> targetArray)
        {
            foreach (var actor in sourceArray)
            {
                var target = targetArray.FirstOrDefault(a => a.Reference.Id == actor.Reference.Id);

                if (target == null)
                {
                    target = new ActorSynchronizationDto
                    {
                        Reference = actor.Reference
                    };
                    targetArray.Insert(actor.Position, target);
                }

                target.Name = actor.Name;
                target.Char = actor.Char;
                target.Color = actor.Color;
                target.Left = actor.Left;
                target.OwnerId = actor.OwnerId;
                target.Tags = actor.Tags;
                target.ParentId = actor.ParentId;
                target.Durability = actor.Durability;
                target.MaxDurability = actor.MaxDurability;
                target.Initiative = actor.Initiative;
                target.InitiativePosition = actor.InitiativePosition;
                target.Height = actor.Height;
                target.Volume = actor.Volume;
                target.FreeVolume = actor.FreeVolume;
                target.PreparationReactions = actor.PreparationReactions;
                target.ActiveReactions = actor.ActiveReactions;
                target.ClearReactions = actor.ClearReactions;
                target.Actions = actor.Actions;
                target.Buffs = actor.Buffs;

                MergeActors(actor.Actors, target.Actors);
            }
        }
    }
}