using System;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Game
{
    public class DailyChangedInfoDto
    {
        public IEnumerable<CharacterForSaleDto> Tavern { get; set; }
    }
}