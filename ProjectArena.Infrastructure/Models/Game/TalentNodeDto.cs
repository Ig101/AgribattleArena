using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Game
{
    public class TalentNodeDto
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public CharacterClass? Class { get; set; }

        public int ClassPoints { get; set; }

        public int Strength { get; set; }

        public int Willpower { get; set; }

        public int Constitution { get; set; }

        public int Speed { get; set; }

        public IEnumerable<string> Prerequisites { get; set; }

        public IEnumerable<string> Exceptions { get; set; }

        public int SkillsAmount { get; set; }
    }
}