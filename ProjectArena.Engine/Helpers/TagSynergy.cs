using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectArena.Engine.Helpers
{
    public struct TagSynergy
    {
        public string SelfTag { get; }

        public string TargetTag { get; }

        public float Mod { get; }

        public TagSynergy(string selfTag, string targetTag, float mod)
        {
            this.SelfTag = string.Intern(selfTag);
            this.TargetTag = string.Intern(targetTag);
            this.Mod = mod;
        }

        public TagSynergy(string targetTag, float mod)
        {
            this.SelfTag = null;
            this.TargetTag = string.Intern(targetTag);
            this.Mod = mod;
        }
    }
}
