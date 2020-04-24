using System;
using System.Collections.Generic;
using System.Text;
using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Tests.Engine.Helpers
{
    public class BasicEngineTester
    {
        protected Scene Scene { get; set; }

        protected List<ISyncEventArgs> SyncMessages { get; set; }

        protected void EventHandler(object sender, ISyncEventArgs e)
        {
            SyncMessages.Add(e);
        }
    }
}
