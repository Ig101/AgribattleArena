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

        public long Id { get; }

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
            long id,
            IEnumerable<ForExternalUse.Generation.ObjectInterfaces.IPlayer> players,
            ForExternalUse.Generation.ISceneGenerator generator,
            ForExternalUse.INativeManager nativeManager,
            ForExternalUse.IVarManager varManager,
            int seed)
        {
            this.IsActive = true;
            this.Id = id;
            this.GameRandom = new Random(seed);
            this.NativeManager = (INativeManager)nativeManager;
            this.VarManager = (IVarManager)varManager;
            ISceneGenerator tempGenerator = (ISceneGenerator)generator;
            this.WinCondition = tempGenerator.WinCondition;
            this.DefeatCondition = tempGenerator.DefeatCondition;
            this.EnemyActorsPrefix = string.Empty;
            this.IdsCounter = 1;
            this.players = new List<Player>();
            this.Actors = new List<Actor>();
            this.Decorations = new List<ActiveDecoration>();
            this.SpecEffects = new List<SpecEffect>();
            this.DeletedActors = new List<Actor>();
            this.DeletedDecorations = new List<ActiveDecoration>();
            this.DeletedEffects = new List<SpecEffect>();
            tempGenerator.GenerateNewScene(this, players, unchecked(seed * (int)id));
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

        public Player CreatePlayer(string id, int? team)
        {
            Player player = new Player(this, id, team);
            players.Add(player);
            return player;
        }

        public Actor CreateActor(Player owner, string nativeName, string roleNativeName, Tile target, float? z)
        {
            return CreateActor(owner, null, nativeName, NativeManager.GetRoleModelNative(roleNativeName), target, z);
        }

        public Actor CreateActor(Player owner, long? externalId, string nativeName, RoleModelNative roleModel, Tile target, float? z)
        {
            if (target.TempObject != null)
            {
                return null;
            }

            Actor actor = new Actor(this, owner, externalId, target, z, NativeManager.GetActorNative(nativeName), roleModel);
            Actors.Add(actor);
            target.ChangeTempObject(actor, true);
            return actor;
        }

        public ActiveDecoration CreateDecoration(Player owner, string nativeName, Tile target, float? z, int? health, TagSynergy[] armor, float? mod)
        {
            if (target.TempObject != null)
            {
                return null;
            }

            ActiveDecoration decoration = new ActiveDecoration(this, owner, target, z, health, armor, NativeManager.GetDecorationNative(nativeName), mod);
            Decorations.Add(decoration);
            target.ChangeTempObject(decoration, true);
            return decoration;
        }

        public SpecEffect CreateEffect(Player owner, string nativeName, Tile target, float? z, float? duration, float? mod)
        {
            SpecEffect effect = new SpecEffect(this, owner, target.X, target.Y, z, NativeManager.GetEffectNative(nativeName), duration, mod);
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
                Tiles[x][y].Native.OnStepAction(this, Tiles[x][y]);
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
            this.ReturnAction(this, new SyncEventArgs(this, ++Version, Helpers.Action.StartGame, GetFullSynchronizationData(true), null, null, null, null));
            EndTurn();
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

                foreach (TileObject obj in Decorations)
                {
                    if (obj.IsAlive && obj.InitiativePosition < minInitiativePosition)
                    {
                        minInitiativePosition = obj.InitiativePosition;
                        newObject = obj;
                    }
                }

                if (newObject != null)
                {
                    this.RemainedTurnTime = newObject.Owner == null || newObject.Owner.TurnsSkipped <= 0 ? VarManager.TurnTimeLimit : VarManager.TurnTimeLimitAfterSkip;
                    this.TempTileObject = newObject;
                    Update(minInitiativePosition);
                    turnStarted = this.TempTileObject.StartTurn();
                    if (!AfterUpdateSynchronization(Helpers.Action.EndTurn, TempTileObject, null, null, null))
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
                    AfterUpdateSynchronization(Helpers.Action.NoActorsDraw, null, null, null, null);
                }
            }
            while (!turnStarted);
        }

        private void Update(float time)
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
                obj.Update(time);
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
                if (player.Status == PlayerStatus.Playing &&
                    (player.TurnsSkipped >= VarManager.SkippedTurnsLimit || DefeatCondition(this, player)))
                {
                    player.Defeat();
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
                lock (lockObject)
                {
                    if (IsActive)
                    {
                        this.RemainedTurnTime -= time;
                        if (RemainedTurnTime <= 0)
                        {
                            IPlayerParentRef player = TempTileObject.Owner;
                            if (player != null)
                            {
                                player.SkipTurn();
                                if (AfterUpdateSynchronization(Helpers.Action.SkipTurn, TempTileObject, null, null, null))
                                {
                                    EndTurn();
                                }
                            }
                            else
                            {
                                EndTurn();
                            }
                        }
                    }
                }
            }
        }

        public bool AfterUpdateSynchronization(Helpers.Action action, TileObject actor, int? actionId, int? targetX, int? targetY)
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

                this.ReturnAction?.Invoke(this, new SyncEventArgs(this, ++Version, Helpers.Action.EndGame, GetSynchronizationDataPlayersOnly(), null, null, null, null));
                IsActive = false;
                return false;
            }

            return true;
        }

        // Actions methods
        private void ApplyActionAfterSkipping()
        {
            RemainedTurnTime += VarManager.TurnTimeLimit - VarManager.TurnTimeLimitAfterSkip;
        }

        public bool DecorationCast(ActiveDecoration actor)
        {
            if (TempTileObject == actor)
            {
                if (actor.Owner?.ActThisTurn() ?? false)
                {
                    ApplyActionAfterSkipping();
                }

                actor.Cast();
                if (AfterUpdateSynchronization(Helpers.Action.Decoration, actor, null, null, null))
                {
                    EndTurn();
                }

                return true;
            }

            return false;
        }

        public bool ActorMove(int actorId, int targetX, int targetY)
        {
            if (TempTileObject.Id == actorId && IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                lock (lockObject)
                {
                    if (TempTileObject.Id == actorId && IsActive)
                    {
                        Actor actor = (Actor)TempTileObject;
                        if (actor.Owner?.ActThisTurn() ?? false)
                        {
                            ApplyActionAfterSkipping();
                        }

                        bool result = actor.Move(Tiles[targetX][targetY]);
                        if (result)
                        {
                            bool actionAvailability = actor.CheckActionAvailability();
                            if (actionAvailability)
                            {
                                Update(0);
                                AfterActionUpdate();
                            }

                            bool afterActionUpdateSynchronization = AfterUpdateSynchronization(Helpers.Action.Move, actor, null, targetX, targetY);
                            if (afterActionUpdateSynchronization && !actionAvailability)
                            {
                                EndTurn();
                            }
                        }

                        return result;
                    }
                }
            }

            return false;
        }

        public bool ActorCast(int actorId, int skillId, int targetX, int targetY)
        {
            if (TempTileObject.Id == actorId && IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                lock (lockObject)
                {
                    if (TempTileObject.Id == actorId && IsActive)
                    {
                        Actor actor = (Actor)TempTileObject;
                        if (actor.Owner?.ActThisTurn() ?? false)
                        {
                            ApplyActionAfterSkipping();
                        }

                        bool result = actor.Cast(skillId, Tiles[targetX][targetY]);
                        if (result)
                        {
                            bool actionAvailability = actor.CheckActionAvailability();
                            if (actionAvailability)
                            {
                                Update(0);
                                AfterActionUpdate();
                            }

                            bool afterActionUpdateSynchronization = AfterUpdateSynchronization(Helpers.Action.Cast, actor, skillId, targetX, targetY);
                            if (afterActionUpdateSynchronization && !actionAvailability)
                            {
                                EndTurn();
                            }
                        }

                        return result;
                    }
                }
            }

            return false;
        }

        public bool ActorAttack(int actorId, int targetX, int targetY)
        {
            if (TempTileObject.Id == actorId && IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                lock (lockObject)
                {
                    if (TempTileObject.Id == actorId && IsActive)
                    {
                        Actor actor = (Actor)TempTileObject;
                        if (actor.Owner?.ActThisTurn() ?? false)
                        {
                            ApplyActionAfterSkipping();
                        }

                        bool result = actor.Attack(Tiles[targetX][targetY]);
                        if (result)
                        {
                            bool actionAvailability = actor.CheckActionAvailability();
                            if (actionAvailability)
                            {
                                Update(0);
                                AfterActionUpdate();
                            }

                            bool afterActionUpdateSynchronization = AfterUpdateSynchronization(Helpers.Action.Attack, actor, null, targetX, targetY);
                            if (afterActionUpdateSynchronization && !actionAvailability)
                            {
                                EndTurn();
                            }
                        }

                        return result;
                    }
                }
            }

            return false;
        }

        public bool ActorWait(int actorId)
        {
            if (TempTileObject.Id == actorId && IsActive)
            {
                lock (lockObject)
                {
                    if (TempTileObject.Id == actorId && IsActive)
                    {
                        Actor actor = (Actor)TempTileObject;
                        if (actor.Owner?.ActThisTurn() ?? false)
                        {
                            ApplyActionAfterSkipping();
                        }

                        bool result = actor.Wait();
                        if (result)
                        {
                            if (AfterUpdateSynchronization(Helpers.Action.Wait, actor, null, null, null))
                            {
                                EndTurn();
                            }
                        }

                        return result;
                    }
                }
            }

            return false;
        }
    }
}
