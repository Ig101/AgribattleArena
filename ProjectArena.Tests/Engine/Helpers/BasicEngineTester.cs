using AgribattleArena.Engine;
using AgribattleArena.Engine.ForExternalUse.Synchronization;
using AgribattleArena.Engine.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgribattleArena.Tests.Engine.Helpers
{
    public class BasicEngineTester
    {
        protected Scene _scene;
        protected List<ISyncEventArgs> _syncMessages;

        protected void EventHandler(object sender, ISyncEventArgs e)
        {
            _syncMessages.Add(e);
        }


    }
}
