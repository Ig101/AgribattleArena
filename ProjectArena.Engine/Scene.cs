using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.NativeManagers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.SceneGenerators;
using ProjectArena.Engine.Synchronizers;
using ProjectArena.Engine.VarManagers;

namespace ProjectArena.Engine
{
    public class Scene : ISceneParentRef, ISceneForSceneGenerator, ForExternalUse.IScene
    {
        public delegate bool DefeatConditionMethod(ISceneParentRef scene, IPlayerParentRef player);

        public delegate bool WinConditionMethod(ISceneParentRef scene);

        public event EventHandler<ForExternalUse.Synchronization.ISyncEventArgs> ReturnAction;

        private readonly object lockObject = new object();

        private readonly List<Player> players;

        private readonly int moveMilliseconds;

        private DateTime? lastMoveTime;

        public bool IsActive { get; private set; }

        public float PassedTime { get; private set; }

        public string EnemyActorsPrefix { get; private set; }

        public DefeatConditionMethod DefeatCondition { get; }

        public WinConditionMethod WinCondition { get; }

        public Random GameRandom { get; }

        public Tile[][] Tiles { get; private set; }

        public List<Actor> Actors { get; }

        public List<ActiveDecoration> Decorations { get; }

        public List<SpecEffect> SpecEffects { get; }

        public List<Actor> DeletedActors { get; private set; }

        public List<ActiveDecoration> DeletedDecorations { get; private set; }

        public List<SpecEffect> DeletedEffects { get; private set; }

        public int IdsCounter { get; private set; }

        public IVarManager VarManager { get; }

        public INativeManager NativeManager { get; }

        public int Version { get; private set; }

        public float RemainedTurnTime { get; private set; }

        public TileObject TempTileObject { get; private set; }

        public Guid Id { get; }

        public int RandomCounter { get; private set; }

        public IEnumerable<Player> Players
        {
            get
            {
                return players;
            }
        }

        public IEnumerable<ForExternalUse.IPlayerShort> ShortPlayers
        {
            get
            {
                return players;
            }
        }

        public Scene(
            Guid id,
            IEnumerable<ForExternalUse.Generation.ObjectInterfaces.IPlayer> players,
            ForExternalUse.Generation.ISceneGenerator generator,
            ForExternalUse.INativeManager nativeManager,
            ForExternalUse.IVarManager varManager,
            int seed,
            int moveMilliseconds)
        {
            this.moveMilliseconds = moveMilliseconds;
            this.IdsCounter = 1;
            this.IsActive = true;
            this.Id = id;
            this.PassedTime = 0;
            this.GameRandom = new Random(seed);
            this.NativeManager = (INativeManager)nativeManager;
            this.VarManager = (IVarManager)varManager;
            ISceneGenerator tempGenerator = (ISceneGenerator)generator;
            this.WinCondition = tempGenerator.WinCondition;
            this.DefeatCondition = tempGenerator.DefeatCondition;
            this.EnemyActorsPrefix = string.Empty;
            this.players = new List<Player>();
            this.Actors = new List<Actor>();
            this.Decorations = new List<ActiveDecoration>();
            this.SpecEffects = new List<SpecEffect>();
            this.DeletedActors = new List<Actor>();
            this.DeletedDecorations = new List<ActiveDecoration>();
            this.DeletedEffects = new List<SpecEffect>();
            tempGenerator.GenerateNewScene(this, players, unchecked(seed * id.GetHashCode()));
        }

        public float GetNextRandom()
        {
            RandomCounter++;
            return (float)GameRandom.NextDouble();
        }

        public int GetNextId()
        {
            int tempId = IdsCounter;
            IdsCounter++;
            return tempId;
        }

        public List<Actor> GetPlayerActors(Player player)
        {
            return Actors.FindAll(x => x.Owner == player);
        }

        public void SetupEnemyActorsPrefix(string prefix)
        {
            this.EnemyActorsPrefix = prefix;
        }

        public IEnumerable<int> GetPlayerActors(string playerId)
        {
            return Actors.FindAll(x => x.Owner != null && x.Owner.Id == playerId).Select(x => x.Id);
        }

        public IEnumerable<int> GetUserActors(string userId)
        {
            return Actors.FindAll(x => x.Owner != null && x.Owner.UserId == userId).Select(x => x.Id);
        }

        // Creation methods
        public Actor ResurrectActor(Actor actor, Tile target, int health)
        {
            if (target.TempObject != null)
            {
                return null;
            }

            actor.DamageModel.Health = health;
            actor.TempTile = target;
            target.ChangeTempObject(actor, true);
            actor.IsAlive = true;
            actor.Affected = true;
            Actors.Add(actor);
            return actor;
        }

        public Tile[][] SetupEmptyTileSet(int width, int height)
        {
            Tile[][] tiles = new Tile[width][];
            for (int i = 0; i < width; i++)
            {
                tiles[i] = new Tile[height];
            }

            this.Tiles = tiles;
            return tiles;
        }

        public Player CreatePlayer(string id, string userId, int? team)
        {
            Player player = new Player(this, id, userId, team);
            players.Add(player);
            return player;
        }

        public Actor CreateActor(Player owner, string nativeName, string roleNativeName, Tile target, string visualization, string enemyVisualization, float? z)
        {
            return CreateActor(owner, null, nativeName, NativeManager.GetRoleModelNative(roleNativeName), target, visualization, enemyVisualization, z);
        }

        public Actor CreateActor(Player owner, Guid? externalId, string nativeName, RoleModelNative roleModel, Tile target, string visualization, string enemyVisualization, float? z)
        {
            if (target.TempObject != null)
            {
                return null;
            }

            Actor actor = new Actor(this, owner, externalId, target, visualization, enemyVisualization, z, NativeManager.GetActorNative(nativeName), roleModel);
            Actors.Add(actor);
            target.ChangeTempObject(actor, true);
            return actor;
        }

        public ActiveDecoration CreateDecoration(Player owner, string nativeName, Tile target, TagSynergy[] armor, string visualization, float? z, int? health, float? mod)
        {
            if (target.TempObject != null)
            {
                return null;
            }

            ActiveDecoration decoration = new ActiveDecoration(this, owner, target, visualization, z, health, armor, NativeManager.GetDecorationNative(nativeName), mod);
            Decorations.Add(decoration);
            target.ChangeTempObject(decoration, true);
            return decoration;
        }

        public SpecEffect CreateEffect(Player owner, string nativeName, Tile target, string visualization, float? z, float? duration, float? mod)
        {
            SpecEffect effect = new SpecEffect(this, owner, target.X, target.Y, visualization, z, NativeManager.GetEffectNative(nativeName), duration, mod);
            SpecEffects.Add(effect);
            return effect;
        }

        public Tile ChangeTile(string nativeName, int x, int y, float? height, IPlayerParentRef owner)
        {
            if (Tiles[x][y] == null)
            {
                return null;
            }

            Tile tile = Tiles[x][y];
            tile.Owner = owner;
            tile.Native = NativeManager.GetTileNative(nativeName);
            tile.Affected = true;
            if (height != null)
            {
                tile.Height = height.Value;
            }

            if (tile.Native.Unbearable && tile.TempObject != null)
            {
                tile.TempObject.Kill();
            }

            if (tile.TempObject != null)
            {
                if (tile.TempObject is Actor actor)
                {
                    actor.BuffManager.RemoveTileBuffs();
                }

                if (!tile.Revealed)
                {
                    tile.Affected = true;
                    tile.Revealed = true;
                }

                tile.Native.OnStepAction(this, Tiles[x][y]);
            }

            return tile;
        }

        public Tile CreateTile(string nativeName, int x, int y, int? height)
        {
            if (Tiles[x][y] != null)
            {
                return null;
            }

            Tile tile = new Tile(this, x, y, NativeManager.GetTileNative(nativeName), height);
            Tiles[x][y] = tile;
            return tile;
        }

        // Sync methods
        private ForExternalUse.Synchronization.ISynchronizer GetSynchronizationDataPlayersOnly()
        {
            return new Synchronizer(
                TempTileObject,
                players,
                new List<Actor>(),
                new List<ActiveDecoration>(),
                new List<SpecEffect>(),
                new List<Actor>(),
                new List<ActiveDecoration>(),
                new List<SpecEffect>(),
                new Point(Tiles.Length, Tiles[0].Length),
                new List<Tile>(),
                RandomCounter);
        }

        private ForExternalUse.Synchronization.ISynchronizer GetSynchronizationData(bool nullify)
        {
            List<Actor> changedActors = Actors.FindAll(x => x.Affected);
            List<ActiveDecoration> changedDecorations = Decorations.FindAll(x => x.Affected);
            List<SpecEffect> changedEffects = SpecEffects.FindAll(x => x.Affected);
            List<Tile> changedTiles = new List<Tile>();
            for (int x = 0; x < Tiles.Length; x++)
            {
                for (int y = 0; y < Tiles[x].Length; y++)
                {
                    if (Tiles[x][y].Affected)
                    {
                        changedTiles.Add(Tiles[x][y]);
                    }
                }
            }

            Synchronizer sync = new Synchronizer(
                TempTileObject,
                players,
                changedActors,
                changedDecorations,
                changedEffects,
                DeletedActors,
                DeletedDecorations,
                DeletedEffects,
                new Point(Tiles.Length, Tiles[0].Length),
                changedTiles,
                RandomCounter);
            if (nullify)
            {
                changedActors.ForEach(x => x.Affected = false);
                changedDecorations.ToList().ForEach(x => x.Affected = false);
                changedEffects.ToList().ForEach(x => x.Affected = false);
                changedTiles.ToList().ForEach(x => x.Affected = false);
                this.DeletedDecorations = new List<ActiveDecoration>();
                this.DeletedActors = new List<Actor>();
                this.DeletedEffects = new List<SpecEffect>();
            }

            return sync;
        }

        private ForExternalUse.Synchronization.ISynchronizer GetSynchronizationData()
        {
            return GetSynchronizationData(false);
        }

        private ForExternalUse.Synchronization.ISynchronizer GetFullSynchronizationData(bool nullify)
        {
            if (nullify)
            {
                Actors.ForEach(x => x.Affected = false);
                Decorations.ToList().ForEach(x => x.Affected = false);
                SpecEffects.ToList().ForEach(x => x.Affected = false);
                for (int x = 0; x < Tiles.Length; x++)
                {
                    for (int y = 0; y < Tiles[x].Length; y++)
                    {
                        Tiles[x][y].Affected = false;
                    }
                }

                this.DeletedDecorations.Clear();
                this.DeletedActors.Clear();
                this.DeletedEffects.Clear();
            }

            return new SynchronizerFull(TempTileObject, players, Actors, Decorations, SpecEffects, Tiles, RandomCounter);
        }

        public ForExternalUse.Synchronization.ISynchronizer GetFullSynchronizationData()
        {
            return GetFullSynchronizationData(false);
        }

        // Updates methods
        public void StartGame()
        {
            this.RemainedTurnTime = 5;
            this.ReturnAction(this, new SyncEventArgs(this, ++Version, Helpers.SceneAction.StartGame, GetFullSynchronizationData(true), null, null, null, null));
        }

        public void EndTurn()
        {
            bool turnStarted;
            do
            {
                TempTileObject?.EndTurn();
                float minInitiativePosition = float.MaxValue;
                TileObject newObject = null;
                foreach (TileObject obj in Actors)
                {
                    if (obj.IsAlive && obj.InitiativePosition < minInitiativePosition)
                    {
                        minInitiativePosition = obj.InitiativePosition;
                        newObject = obj;
                    }
                }

                foreach (ActiveDecoration obj in Decorations)
                {
                    if (obj.IsAlive && obj.InitiativePosition < minInitiativePosition && obj.Native.Action != null)
                    {
                        minInitiativePosition = obj.InitiativePosition;
                        newObject = obj;
                    }
                }

                if (newObject != null)
                {
                    this.RemainedTurnTime = VarManager.TurnTimeLimit;
                    this.TempTileObject = newObject;
                    Update(minInitiativePosition);
                    turnStarted = this.TempTileObject.StartTurn();
                    if (!AfterUpdateSynchronization(Helpers.SceneAction.EndTurn, TempTileObject, null, null, null))
                    {
                        turnStarted = true;
                    }
                    else if (this.TempTileObject is ActiveDecoration decoration)
                    {
                        DecorationCast(decoration);
                    }
                }
                else
                {
                    turnStarted = true;
                    AfterUpdateSynchronization(Helpers.SceneAction.NoActorsDraw, null, null, null, null);
                }
            }
            while (!turnStarted);
        }

        public bool ActorWait()
        {
            Actor actor = (Actor)TempTileObject;

            bool result = actor.Wait();
            if (result)
            {
                lastMoveTime = null;
                EndTurn();
            }

            return result;
        }

        private void Update(float time, Actor specificActor = null)
        {
            PassedTime += time;
            for (int x = 0; x < Tiles.Length; x++)
            {
                for (int y = 0; y < Tiles[x].Length; y++)
                {
                    Tiles[x][y].Update(time);
                }
            }

            foreach (TileObject obj in Actors)
            {
                if (specificActor == null || obj == specificActor)
                {
                    obj.Update(time);
                }
            }

            foreach (TileObject obj in Decorations)
            {
                obj.Update(time);
            }

            foreach (SpecEffect eff in SpecEffects)
            {
                eff.Update(time);
            }
        }

        private void AfterActionUpdate()
        {
            foreach (Player player in players)
            {
                if (player.Status == PlayerStatus.Playing && DefeatCondition(this, player))
                {
                    player.Defeat(false);
                }
            }

            for (int i = 0; i < Actors.Count; i++)
            {
                if (!Actors[i].IsAlive)
                {
                    Actors[i].TempTile.RemoveTempObject();
                    DeletedActors.Add(Actors[i]);
                    Actors.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < Decorations.Count; i++)
            {
                if (!Decorations[i].IsAlive)
                {
                    Decorations[i].TempTile.RemoveTempObject();
                    DeletedDecorations.Add(Decorations[i]);
                    Decorations.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < SpecEffects.Count; i++)
            {
                if (!SpecEffects[i].IsAlive)
                {
                    DeletedEffects.Add(SpecEffects[i]);
                    SpecEffects.RemoveAt(i);
                    i--;
                }
            }
        }

        public void UpdateTime(float time)
        {
            if (IsActive)
            {
                this.RemainedTurnTime -= time;
                if (RemainedTurnTime <= 0)
                {
                    lock (lockObject)
                    {
                        if (IsActive)
                        {
                            if (TempTileObject == null)
                            {
                                EndTurn();
                            }
                            else
                            {
                                ActorWait();
                            }
                        }
                    }
                }
            }
        }

        public bool AfterUpdateSynchronization(Helpers.SceneAction action, TileObject actor, int? actionId, int? targetX, int? targetY)
        {
            AfterActionUpdate();
            this.ReturnAction?.Invoke(this, new SyncEventArgs(this, ++Version, action, GetSynchronizationData(true), actor?.Id, actionId, targetX, targetY));
            if (WinCondition(this))
            {
                foreach (Player player in players)
                {
                    if (player.Status == PlayerStatus.Playing)
                    {
                        player.Victory();
                    }
                }

                this.ReturnAction?.Invoke(this, new SyncEventArgs(this, ++Version, Helpers.SceneAction.EndGame, GetSynchronizationDataPlayersOnly(), null, null, null, null));
                IsActive = false;
                return false;
            }

            return true;
        }

        // Actions methods
        public bool LeaveScene(string userId)
        {
            var players = this.players.Where(x => x.UserId == userId).ToList();
            var success = false;
            var needNewAction = false;
            foreach (var player in players)
            {
                if (!player.Left)
                {
                    success = true;
                    player.Left = true;
                    if (player.Status == PlayerStatus.Playing)
                    {
                        needNewAction = true;
                        player.Defeat(true);
                    }
                }
            }

            if (needNewAction)
            {
                AfterUpdateSynchronization(Helpers.SceneAction.Leave, TempTileObject, null, null, null);
            }

            return success;
        }

        public bool DecorationCast(ActiveDecoration actor)
        {
            if (TempTileObject == actor)
            {
                actor.Cast();
                if (AfterUpdateSynchronization(Helpers.SceneAction.Decoration, actor, null, null, null))
                {
                    EndTurn();
                }

                return true;
            }

            return false;
        }

        private bool CheckMoveTime()
        {
            return lastMoveTime == null || (DateTime.Now - lastMoveTime.Value).TotalMilliseconds >= moveMilliseconds;
        }

        public bool ActorMove(int actorId, int targetX, int targetY)
        {
            if (TempTileObject?.Id == actorId && IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length && CheckMoveTime())
            {
                lock (lockObject)
                {
                    if (TempTileObject?.Id == actorId && IsActive && CheckMoveTime())
                    {
                        Actor actor = (Actor)TempTileObject;

                        bool result = actor.Move(Tiles[targetX][targetY]);
                        if (result)
                        {
                            lastMoveTime = DateTime.Now;
                            bool actionAvailability = actor.CheckActionAvailability();
                            if (actionAvailability)
                            {
                                Update(0, actor);
                                AfterActionUpdate();
                            }

                            bool afterActionUpdateSynchronization = AfterUpdateSynchronization(Helpers.SceneAction.Move, actor, null, targetX, targetY);
                            if (afterActionUpdateSynchronization && !actionAvailability)
                            {
                                EndTurn();
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool ActorCast(int actorId, int skillId, int targetX, int targetY)
        {
            if (TempTileObject?.Id == actorId && IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length && CheckMoveTime())
            {
                lock (lockObject)
                {
                    if (TempTileObject?.Id == actorId && IsActive && CheckMoveTime())
                    {
                        Actor actor = (Actor)TempTileObject;

                        bool result = actor.Cast(skillId, Tiles[targetX][targetY]);
                        if (result)
                        {
                            bool actionAvailability = actor.CheckActionAvailability();
                            if (actionAvailability)
                            {
                                Update(0, actor);
                                AfterActionUpdate();
                            }

                            bool afterActionUpdateSynchronization = AfterUpdateSynchronization(Helpers.SceneAction.Cast, actor, skillId, targetX, targetY);
                            if (afterActionUpdateSynchronization && !actionAvailability)
                            {
                                EndTurn();
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool ActorAttack(int actorId, int targetX, int targetY)
        {
            if (TempTileObject?.Id == actorId && IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length && CheckMoveTime())
            {
                lock (lockObject)
                {
                    if (TempTileObject?.Id == actorId && IsActive && CheckMoveTime())
                    {
                        Actor actor = (Actor)TempTileObject;

                        bool result = actor.Attack(Tiles[targetX][targetY]);
                        if (result)
                        {
                            bool actionAvailability = actor.CheckActionAvailability();
                            if (actionAvailability)
                            {
                                Update(0, actor);
                                AfterActionUpdate();
                            }

                            bool afterActionUpdateSynchronization = AfterUpdateSynchronization(Helpers.SceneAction.Attack, actor, null, targetX, targetY);
                            if (afterActionUpdateSynchronization && !actionAvailability)
                            {
                                EndTurn();
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool SkipTurn(int actorId)
        {
            if (TempTileObject?.Id == actorId && this.VarManager.CanEndTurnPrematurely && IsActive)
            {
                lock (lockObject)
                {
                    if (TempTileObject?.Id == actorId && IsActive)
                    {
                        ActorWait();
                    }
                }
            }

            return false;
        }
    }
}
