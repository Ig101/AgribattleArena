using System;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;

namespace ProjectArena.Domain
{
    internal static class DefaultBotRostersRegistry
    {
        public static void UseGlassCannonPartyBotRoster(this GameContext gameContext, string botId)
        {
            var rosterId = Guid.NewGuid().ToString();
            gameContext.Rosters.InsertOne(new Roster()
            {
                Id = rosterId,
                UserId = botId,
                Experience = 0,
                Seed = botId.GetHashCode(),
                TavernCapacity = 0,
                BoughtPatrons = new int[0]
            });
            gameContext.Characters.Insert(new[]
            {
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotRanger",
                    ChosenTalents = new[]
                    {
                        24013,
                        23013,
                        23014,
                        22014,
                        22015,
                        22016,
                        22017,
                        21017,
                        21015,
                        20015,
                        19015,
                        18015,
                        17015,
                        17016,
                        17017,
                        17018,
                        21018,
                        21019,
                        22019,
                        23019,
                        24019,
                        25019,
                        26019,
                        17014,
                        16014,
                        15014,
                        21014,
                        25020,
                        26020,
                        25013
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotFighter",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
                        24011,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        20011,
                        19011,
                        18011,
                        17011,
                        16011,
                        16010,
                        17014,
                        17015,
                        17016,
                        17017,
                        17018,
                        25011,
                        16013,
                        25012,
                        16014,
                        15014,
                        14014,
                        14013,
                        14012,
                        13012,
                        13011
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotMage",
                    ChosenTalents = new[]
                    {
                        25012,
                        26012,
                        27012,
                        28012,
                        28011,
                        29011,
                        30011,
                        31011,
                        31010,
                        31009,
                        31008,
                        31007,
                        31006,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        26007,
                        27007,
                        32011,
                        32010,
                        33010,
                        34010,
                        28013,
                        30009,
                        29009,
                        27006,
                        27005,
                        26005
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotArchitect",
                    ChosenTalents = new[]
                    {
                        25012,
                        25013,
                        25014,
                        26014,
                        27014,
                        27015,
                        28015,
                        28016,
                        29016,
                        30016,
                        31016,
                        31015,
                        32015,
                        32014,
                        33014,
                        34014,
                        34013,
                        26012,
                        27012,
                        28012,
                        28011,
                        29011,
                        23012,
                        30011,
                        31011,
                        32011,
                        34015,
                        34016,
                        34017,
                        33017
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotEnchanter",
                    ChosenTalents = new[]
                    {
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        24011,
                        22009,
                        22008,
                        22007,
                        24013,
                        25013,
                        25012,
                        26012,
                        27012,
                        28012,
                        28013,
                        29013,
                        30013,
                        28011,
                        29011,
                        30011,
                        31011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        23007
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotEnchanter",
                    ChosenTalents = new[]
                    {
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        25012,
                        26012,
                        27012,
                        28012,
                        28011,
                        29011,
                        30011,
                        31011,
                        31010,
                        32010,
                        33010,
                        34010,
                        34009,
                        34008,
                        34007,
                        33007,
                        33006,
                        32006,
                        25011,
                        25010,
                        23012,
                        26010,
                        20008
                    }
                }
            });
        }
    }
}