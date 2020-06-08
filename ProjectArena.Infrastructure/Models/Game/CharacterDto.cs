using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Game
{
    public class CharacterDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsKeyCharacter { get; set; }

        public IEnumerable<PointDto> Talents { get; set; }
    }
}