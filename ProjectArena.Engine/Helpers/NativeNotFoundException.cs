using System;
using System.Collections.Generic;
using System.Text;

namespace AgribattleArena.Engine.Helpers
{
    internal class NativeNotFoundException : Exception
    {
        public NativeNotFoundException(string type, string id)
            : base(type + " with name " + id + " not found.")
        {
            this.Data.Add("id", id);
            this.Data.Add("type", type);
        }
    }
}
