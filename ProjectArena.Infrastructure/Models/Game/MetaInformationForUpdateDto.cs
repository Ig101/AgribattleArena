using System;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Game
{
    public class MetaInformationForUpdateDto
    {
        public int Incrementor { get; set; }

        public int Turn { get; set; }

        public GameState GameState { get; set; }
    }
}