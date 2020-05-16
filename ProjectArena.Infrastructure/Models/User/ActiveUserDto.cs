using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Game;

namespace ProjectArena.Infrastructure.Models.User
{
    public class ActiveUserDto : UserDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public UserState State { get; set; }

        public int Experience { get; set; }

        public IEnumerable<CharacterDto> Roster { get; set; }

        public IEnumerable<CharacterForSaleDto> Tavern { get; set; }

        public IEnumerable<TalentNodeDto> TalentsMap { get; set; }
    }
}