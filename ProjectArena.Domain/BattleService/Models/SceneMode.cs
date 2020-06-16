using ProjectArena.Domain.BattleService.Helpers;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.Generation;

namespace ProjectArena.Domain.BattleService.Models
{
    public class SceneMode
    {
        public int MaxPlayers { get; set; }

        public int? TimeTillBot { get; set; }

        public bool AllowMultiEnqueue { get; set; }

        public ISceneGenerator Generator { get; set; }

        public IVarManager VarManager { get; set; }

        public ProcessBattleResult BattleResultProcessor { get; set; }
    }
}