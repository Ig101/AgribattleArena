using System.Collections.Generic;

namespace ProjectArena.Domain.Identity
{
    public class DefaultUsersSettings
    {
        public DefaultLoginPassword Root { get; set; }

        public IEnumerable<DefaultLoginPassword> Bots { get; set; }
    }
}