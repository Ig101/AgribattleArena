using ProjectArena.Domain.Mongo;

namespace ProjectArena.Domain.Game
{
    public class GameContext : BaseMongoContext
    {
        public GameContext(IMongoConnection connection)
            : base(connection)
        {
        }
    }
}