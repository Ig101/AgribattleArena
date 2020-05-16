using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Domain.Registry.Migrations
{
    public class M0000_AddDefaultTalents : IContentMigration
    {
        private double? GetRangeTo(int x, int y, CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Fighter:
                    if (x >= 0)
                    {
                        return null;
                    }

                    return Math.Abs(y);
                case CharacterClass.Enchanter:
                    if (y >= 0)
                    {
                        return null;
                    }
                    else if (Math.Abs(x) > Math.Abs(y))
                    {
                        var range = Math.Abs(x + y) / Math.Sqrt(2);
                    }
                    else if (Math.Abs(x) < Math.Abs(y))
                    {
                        var range = Math.Abs(x + y);
                    }

                    return 0;
                case CharacterClass.Bloodletter:
                    if (y >= 0)
                    {
                        return null;
                    }
                    else if (Math.Abs(x) > Math.Abs(y))
                    {
                        var range = Math.Abs(x - y) / Math.Sqrt(2);
                    }
                    else if (Math.Abs(x) < Math.Abs(y))
                    {
                        var range = Math.Abs(x - y);
                    }

                    return 0;
                case CharacterClass.Mistcaller:
                    if (x <= 0)
                    {
                        return null;
                    }

                    return Math.Abs(y);
                case CharacterClass.Architect:
                    if (y <= 0)
                    {
                        return null;
                    }
                    else if (Math.Abs(x) > Math.Abs(y))
                    {
                        var range = Math.Abs(x + y) / Math.Sqrt(2);
                    }
                    else if (Math.Abs(x) < Math.Abs(y))
                    {
                        var range = Math.Abs(x + y);
                    }

                    return 0;
                case CharacterClass.Ranger:
                    if (y <= 0)
                    {
                        return null;
                    }
                    else if (Math.Abs(x) > Math.Abs(y))
                    {
                        var range = Math.Abs(x - y) / Math.Sqrt(2);
                    }
                    else if (Math.Abs(x) < Math.Abs(y))
                    {
                        var range = Math.Abs(x - y);
                    }

                    return 0;
            }

            return null;
        }

        public void Up(RegistryContext context)
        {
            var random = new Random(9101996);
            var nodes = new List<TalentNode>();
            var defaultNodes = new[]
            {
                (id: "Sw", name: "Cruelty", strength: 1, willpower: -1, constitution: 0, speed: 0),
                (id: "Si", name: "Power", strength: 1, willpower: 0, constitution: 0, speed: -1),
                (id: "Sc", name: "Might", strength: 1, willpower: 0, constitution: -1, speed: 0),
                (id: "Ws", name: "Focus", strength: -1, willpower: 1, constitution: 0, speed: 0),
                (id: "Wi", name: "Will", strength: 0, willpower: 1, constitution: 0, speed: -1),
                (id: "Wc", name: "Zealotry", strength: 0, willpower: 1, constitution: -1, speed: 0),
                (id: "Is", name: "Quickness", strength: -1, willpower: 0, constitution: 0, speed: 1),
                (id: "Iw", name: "Dexterity", strength: 0, willpower: -1, constitution: 0, speed: 1),
                (id: "Ic", name: "Agility", strength: 0, willpower: 0, constitution: -1, speed: 1),
                (id: "Cs", name: "Endurance", strength: -1, willpower: 0, constitution: 1, speed: 0),
                (id: "Cw", name: "Toughness", strength: 0, willpower: -1, constitution: 1, speed: 0),
                (id: "Ci", name: "Resilience", strength: 0, willpower: 0, constitution: 1, speed: -1),
            };

            var fighterNodes = new[] { "Sw", "Si", "Ci", "Cw" };
            var enchanterNodes = new[] { "Si", "Sc", "Wi", "Wc" };
            var bloodletterNodes = new[] { "Ws", "Wi", "Cs", "Ci" };
            var mistcallerNodes = new[] { "Ws", "Wc", "Is", "Ic" };
            var architectNodes = new[] { "Is", "Iw", "Cs", "Cw" };
            var rangerNodes = new[] { "Iw", "Ic", "Sw", "Sc" };

            for (int x = 0; x < 49; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    var realX = x - 24;
                    var realY = y - 12;
                    var firstCheck = random.NextDouble();
                    var secondCheck = random.NextDouble();
                    if (Math.Abs(realX) + Math.Abs(realY) > 24 || (realY == 0 && realX == 0))
                    {
                        continue;
                    }

                    var ranges = new List<(CharacterClass characterClass, double range)>();
                    for (int i = 0; i < 6; i++)
                    {
                        var range = GetRangeTo(realX, realY, (CharacterClass)i);
                        if (range.HasValue)
                        {
                            ranges.Add((characterClass: (CharacterClass)i, range.Value));
                        }
                    }

                    var firstRange = ranges[0];
                    var firstPosition = 0;
                    for (int i = 1; i < ranges.Count; i++)
                    {
                        if (firstRange.range < ranges[i].range)
                        {
                            firstRange = ranges[i];
                            firstPosition = i;
                        }
                    }

                    ranges.RemoveAt(firstPosition);
                    var secondRange = firstRange;
                    if (ranges.Count > 0)
                    {
                        secondRange = ranges[0];
                        for (int i = 1; i < ranges.Count; i++)
                        {
                            if (secondRange.range < ranges[i].range)
                            {
                                secondRange = ranges[i];
                            }
                        }
                    }

                    firstRange.range = 30 - firstRange.range;
                    secondRange.range = 30 - secondRange.range;

                    var firstRangeProbability = firstRange.range / (firstRange.range + secondRange.range);

                    var currentClass = firstCheck <= firstRangeProbability ? firstRange.characterClass : secondRange.characterClass;
                    var nodesList = fighterNodes;
                    switch (currentClass)
                    {
                        case CharacterClass.Enchanter:
                            nodesList = enchanterNodes;
                            break;
                        case CharacterClass.Bloodletter:
                            nodesList = bloodletterNodes;
                            break;
                        case CharacterClass.Mistcaller:
                            nodesList = mistcallerNodes;
                            break;
                        case CharacterClass.Ranger:
                            nodesList = rangerNodes;
                            break;
                        case CharacterClass.Architect:
                            nodesList = architectNodes;
                            break;
                    }

                    secondCheck *= nodesList.Length;
                    int choseNodeId = secondCheck >= nodesList.Length ? nodesList.Length - 1 : (int)secondCheck;
                    var (id, name, strength, willpower, constitution, speed) = defaultNodes.First(x => x.id == nodesList[choseNodeId]);
                    nodes.Add(new TalentNode
                    {
                        Position = (x * 1000) + y,
                        Id = id,
                        Name = name,
                        Class = null,
                        ClassPoints = 0,
                        UniqueDescription = null,
                        UniqueAction = null,
                        StrengthModifier = strength,
                        WillpowerModifier = willpower,
                        ConstitutionModifier = constitution,
                        SpeedModifier = speed,
                        Exceptions = new string[0]
                    });
                }
            }

            context.TalentMap.Insert(nodes);
            context.ApplyChangesAsync().Wait();
        }
    }
}