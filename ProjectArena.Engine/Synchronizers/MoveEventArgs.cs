using System;
using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.Synchronizers
{
    public class MoveEventArgs : EventArgs, IMoveEventArgs
    {
        public IScene Scene { get; }

        public IEnumerable<IMoveInfo> MoveDefinition { get; }

        public MoveEventArgs(Scene scene, IEnumerable<MoveInfo> moveInfo)
        {
            Scene = scene;
            MoveDefinition = moveInfo;
        }
    }
}
