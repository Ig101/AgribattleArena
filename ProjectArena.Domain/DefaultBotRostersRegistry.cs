using System;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;

namespace ProjectArena.Domain
{
    internal static class DefaultBotRostersRegistry
    {
        public static void UseTankRDDRDDBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotTank",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
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
                        17014,
                        16014,
                        16013,
                        15014,
                        14014,
                        25012,
                        25011,
                        24011,
                        14013,
                        14012,
                        13012,
                        12012,
                        11012,
                        17015,
                        17016,
                        17017,
                        17018
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotArcher",
                    ChosenTalents = new[]
                    {
                        23012,
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
                        23016,
                        21018,
                        21019,
                        20019,
                        19019,
                        16018,
                        15018,
                        15017,
                        17014,
                        16014,
                        16013,
                        21014,
                        25013
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
                        32010,
                        32011,
                        30009,
                        29009,
                        28009,
                        27009,
                        27010,
                        28013,
                        29013,
                        30013,
                        31013,
                        32013,
                        32014,
                        33010,
                        34010,
                        34009,
                        34008,
                        31006
                    }
                }
            });
        }

        public static void UseTankMDDRDDBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotTank",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
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
                        17014,
                        16014,
                        16013,
                        15014,
                        14014,
                        25012,
                        25011,
                        24011,
                        14013,
                        14012,
                        13012,
                        12012,
                        11012,
                        17015,
                        17016,
                        17017,
                        17018
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotArcher",
                    ChosenTalents = new[]
                    {
                        23012,
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
                        23016,
                        21018,
                        21019,
                        20019,
                        19019,
                        16018,
                        15018,
                        15017,
                        17014,
                        16014,
                        16013,
                        21014,
                        25013
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotCharger",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        17014,
                        17015,
                        17016,
                        17017,
                        17018,
                        16013,
                        16014,
                        23013,
                        17019,
                        18019,
                        16019,
                        16018,
                        15018,
                        15017,
                        14017,
                        14016,
                        14015,
                        18015,
                        19015,
                        20015,
                        21015,
                        23014
                    }
                }
            });
        }

        public static void UseThreeSummonersBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotSummoner",
                    ChosenTalents = new[]
                    {
                        25012,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
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
                        25011,
                        25010,
                        26010,
                        31006,
                        26009,
                        26008,
                        30009,
                        26007,
                        25008,
                        24008
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotSummoner",
                    ChosenTalents = new[]
                    {
                        25012,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
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
                        25011,
                        25010,
                        26010,
                        31006,
                        26009,
                        26008,
                        30009,
                        26007,
                        25008,
                        24008
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotSummoner",
                    ChosenTalents = new[]
                    {
                        25012,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
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
                        25011,
                        25010,
                        26010,
                        31006,
                        26009,
                        26008,
                        30009,
                        26007,
                        25008,
                        24008
                    }
                }
            });
        }

        public static void UseThreeMDDBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotCharger",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        17014,
                        17015,
                        17016,
                        17017,
                        17018,
                        16013,
                        16014,
                        23013,
                        17019,
                        18019,
                        16019,
                        16018,
                        15018,
                        15017,
                        14017,
                        14016,
                        14015,
                        18015,
                        19015,
                        20015,
                        21015,
                        23014
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotEmpowerer",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        23013,
                        23014,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        20011,
                        19011,
                        18011,
                        17011,
                        16011,
                        16010,
                        15010,
                        14010,
                        14011,
                        22014,
                        24013,
                        14009,
                        14008
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotEmpowerer",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        23013,
                        23014,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        20011,
                        19011,
                        18011,
                        17011,
                        16011,
                        16010,
                        15010,
                        14010,
                        14011,
                        22014,
                        24013,
                        14009,
                        14008
                    }
                }
            });
        }

        public static void UseThreeTanksBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotTank",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
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
                        17014,
                        16014,
                        16013,
                        15014,
                        14014,
                        25012,
                        25011,
                        24011,
                        14013,
                        14012,
                        13012,
                        12012,
                        11012,
                        17015,
                        17016,
                        17017,
                        17018
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotTank",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
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
                        17014,
                        16014,
                        16013,
                        15014,
                        14014,
                        25012,
                        25011,
                        24011,
                        14013,
                        14012,
                        13012,
                        12012,
                        11012,
                        17015,
                        17016,
                        17017,
                        17018
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
                        25012,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        26012,
                        27012,
                        28012,
                        28011,
                        29011,
                        30011,
                        31011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        26007,
                        25008,
                        24008,
                        28013,
                        29013,
                        30013,
                        24007,
                        23007,
                        24006
                    }
                }
            });
        }

        public static void UseThreeMagesBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotEnchanter",
                    ChosenTalents = new[]
                    {
                        25012,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        26012,
                        27012,
                        28012,
                        28011,
                        29011,
                        30011,
                        31011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        26007,
                        25008,
                        24008,
                        28013,
                        29013,
                        30013,
                        24007,
                        23007,
                        24006
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
                        26009,
                        28013,
                        29013,
                        30013,
                        31010,
                        31009,
                        31008,
                        31007,
                        30009,
                        29009,
                        28009,
                        27009,
                        27010,
                        24013,
                        32011,
                        32010,
                        33010,
                        34010,
                        34009,
                        34008,
                        34007,
                        33007
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotMageSummoner",
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
                        32010,
                        32011,
                        30009,
                        29009,
                        28009,
                        27009,
                        27010,
                        28013,
                        29013,
                        30013,
                        31013,
                        32013,
                        32014,
                        33010,
                        34010,
                        34009,
                        34008,
                        31006
                    }
                }
            });
        }
    }
}