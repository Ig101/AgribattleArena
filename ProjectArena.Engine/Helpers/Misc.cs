using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectArena.Engine.Helpers
{
    public enum PlayerStatus
    {
        Playing,
        Victorious,
        Defeated
    }

    public enum Action
    {
        Move,
        Attack,
        Cast,
        Wait,
        Decoration,
        EndTurn,
        EndGame,
        SkipTurn,
        StartGame,
        NoActorsDraw
    }

    public static class Misc
    {
        public static float RangeBetween(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        }

        public static float AngleBetween(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Atan2(y2 - y1, x2 - x1);
        }
    }
}
