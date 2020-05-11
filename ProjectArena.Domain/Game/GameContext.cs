using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.Mongo;

namespace ProjectArena.Domain.Game
{
    public class GameContext : BaseMongoContext
    {
        public IRepository<Character> Characters { get; set; }

        public IRepository<Roster> Rosters { get; set; }

        public GameContext(IMongoConnection connection)
            : base(connection)
        {
            Characters = InitializeRepository<Character>();
            Rosters = InitializeRepository<Roster>();
        }
    }
}