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
                }
            });
        }

        public static void UseUltraMobilePartyBotRoster(this GameContext gameContext, string botId)
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
                        24013,
                        23013,
                        23014,
                        22014,
                        22015,
                        22016,
                        24019,
                        25019,
                        26019,
                        17014,
                        16014,
                        25013,
                        24018,
                        24017,
                        24016,
                        23016,
                        23012,
                        22012,
                        21012,
                        23011,
                        23010,
                        22010,
                        20012,
                        20013,
                        19013,
                        25012,
                        18013,
                        17013,
                        16013,
                        21010
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotOneshoter",
                    ChosenTalents = new[]
                    {
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        17014,
                        17015,
                        17016,
                        16013,
                        16014,
                        23013,
                        23014,
                        22014,
                        22015,
                        22016,
                        23016,
                        24016,
                        24017,
                        24018,
                        24019,
                        25019,
                        26019,
                        21014,
                        21015,
                        20015,
                        19015,
                        18015,
                        23011,
                        23010,
                        22010
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
                        32010,
                        33010,
                        34010,
                        34009,
                        34008,
                        34007,
                        25011,
                        20008,
                        32011,
                        22009,
                        22008,
                        22007,
                        23007,
                        24007,
                        24006
                    }
                }
            });
        }

        public static void UseSummonersPartyBotRoster(this GameContext gameContext, string botId)
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
                        23012,
                        24011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        25008,
                        24008,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        27009,
                        28009,
                        29009,
                        30009,
                        31009,
                        31008,
                        31007,
                        31006,
                        24013,
                        25013,
                        25014,
                        26014,
                        23007,
                        27014,
                        27015,
                        28015
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
                        23012,
                        24011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        25008,
                        24008,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        27009,
                        28009,
                        29009,
                        30009,
                        31009,
                        31008,
                        31007,
                        31006,
                        24013,
                        25013,
                        25014,
                        26014,
                        23007,
                        27014,
                        27015,
                        28015
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
                        23012,
                        24011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        25008,
                        24008,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        27009,
                        28009,
                        29009,
                        30009,
                        31009,
                        31008,
                        31007,
                        31006,
                        24013,
                        25013,
                        25014,
                        26014,
                        23007,
                        27014,
                        27015,
                        28015
                    }
                }
            });
        }

        public static void UseAmbushersPartyBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotArchitect",
                    ChosenTalents = new[]
                    {
                        25012,
                        23012,
                        25013,
                        25014,
                        26014,
                        27014,
                        27015,
                        28015,
                        28016,
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
                        31009,
                        31008,
                        30009,
                        29009,
                        31007,
                        34010,
                        32011,
                        28009,
                        27009,
                        29016,
                        30016
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
                        23012,
                        25013,
                        25014,
                        26014,
                        27014,
                        27015,
                        28015,
                        28016,
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
                        31009,
                        31008,
                        30009,
                        29009,
                        31007,
                        34010,
                        32011,
                        28009,
                        27009,
                        29016,
                        30016
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
                        23012,
                        25013,
                        25014,
                        26014,
                        27014,
                        27015,
                        28015,
                        28016,
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
                        31009,
                        31008,
                        30009,
                        29009,
                        31007,
                        34010,
                        32011,
                        28009,
                        27009,
                        29016,
                        30016
                    }
                }
            });
        }

        public static void UseTanksPartyBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotHealer",
                    ChosenTalents = new[]
                    {
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        25011,
                        20008,
                        22009,
                        22008,
                        22007,
                        23007,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        24013,
                        25010,
                        26010,
                        26009,
                        27010,
                        27009,
                        28009,
                        29009,
                        30009,
                        24008,
                        25008,
                        25005
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
                        17014,
                        16014,
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        16013,
                        24011,
                        25011,
                        25010,
                        26010,
                        15014,
                        14014,
                        14013,
                        20011,
                        19011,
                        18011,
                        17011,
                        16011,
                        16010,
                        16009,
                        17009,
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
                    Name = "BotFighter",
                    ChosenTalents = new[]
                    {
                        17014,
                        16014,
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        16013,
                        24011,
                        25011,
                        25010,
                        26010,
                        15014,
                        14014,
                        14013,
                        20011,
                        19011,
                        18011,
                        17011,
                        16011,
                        16010,
                        16009,
                        17009,
                        17015,
                        17016,
                        17017,
                        17018
                    }
                }
            });
        }

        public static void UseDefaultPartyBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotHealer",
                    ChosenTalents = new[]
                    {
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        25011,
                        20008,
                        22009,
                        22008,
                        22007,
                        23007,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        24013,
                        25010,
                        26010,
                        26009,
                        27010,
                        27009,
                        28009,
                        29009,
                        30009,
                        24008,
                        25008,
                        25005
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
                        17014,
                        16014,
                        23012,
                        22012,
                        21012,
                        20012,
                        20013,
                        19013,
                        18013,
                        17013,
                        16013,
                        24011,
                        25011,
                        25010,
                        26010,
                        15014,
                        14014,
                        14013,
                        20011,
                        19011,
                        18011,
                        17011,
                        16011,
                        16010,
                        16009,
                        17009,
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
                    Name = "BotRanger",
                    ChosenTalents = new[]
                    {
                        23011,
                        24013,
                        25012,
                        26012,
                        27012,
                        28012,
                        28011,
                        29011,
                        30011,
                        31011,
                        32011,
                        31010,
                        31009,
                        31008,
                        31007,
                        31006,
                        24011,
                        25013,
                        25014,
                        32010,
                        33010,
                        30009,
                        29009,
                        28009,
                        27009,
                        28013,
                        29013,
                        30013,
                        26009,
                        26008
                    }
                }
            });
        }

        public static void UseBloodPartyBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotHealer",
                    ChosenTalents = new[]
                    {
                        23012,
                        24011,
                        25011,
                        25010,
                        26010,
                        26009,
                        26008,
                        25008,
                        24008,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        24013,
                        25013,
                        25014,
                        26014,
                        23007,
                        27014,
                        27015,
                        28015,
                        26007,
                        26015,
                        28016,
                        29016,
                        29017,
                        29018,
                        22012,
                        21012
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
                        32011,
                        31010,
                        31009,
                        31008,
                        31007,
                        32010,
                        30009,
                        29009,
                        28009,
                        27009,
                        28013,
                        29013,
                        26009,
                        26008,
                        25011,
                        25010,
                        26010,
                        26007,
                        27007,
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
                        30009,
                        29009,
                        28009,
                        27009,
                        28013,
                        29013,
                        26009,
                        26008,
                        25011,
                        25010,
                        26010,
                        26007,
                        27007,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008
                    }
                }
            });
        }

        public static void UseEnchantedPartyBotRoster(this GameContext gameContext, string botId)
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
                    Name = "BotHealer",
                    ChosenTalents = new[]
                    {
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        25011,
                        20008,
                        22009,
                        22008,
                        22007,
                        23007,
                        24007,
                        24006,
                        24005,
                        23005,
                        22005,
                        24013,
                        25010,
                        26010,
                        26009,
                        27010,
                        27009,
                        28009,
                        29009,
                        30009,
                        24008,
                        25008,
                        25005
                    }
                },
                new Character()
                {
                    Id = Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = true,
                    RosterId = rosterId,
                    Name = "BotMistRanger",
                    ChosenTalents = new[]
                    {
                        25011,
                        25010,
                        26010,
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        19008,
                        18008,
                        17008,
                        17009,
                        16009,
                        16010,
                        15010,
                        14010,
                        14011,
                        24013,
                        23013,
                        23014,
                        22014,
                        22015,
                        22016,
                        22017,
                        21017,
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
                    Name = "BotMistFighter",
                    ChosenTalents = new[]
                    {
                        24011,
                        23011,
                        23010,
                        22010,
                        21010,
                        21009,
                        20009,
                        20008,
                        25011,
                        23012,
                        19008,
                        18008,
                        17008,
                        17009,
                        16009,
                        16010,
                        15010,
                        14010,
                        14011,
                        25010,
                        26010,
                        26009,
                        27009,
                        28009,
                        29009,
                        27010,
                        24013,
                        26008,
                        26007,
                        25008
                    }
                }
            });
        }
    }
}