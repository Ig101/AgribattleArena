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
    public class Scene : ISceneForSceneGenerator, ForExternalUse.IScene
    {
        public delegate bool DefeatConditionMethod(Scene scene, Player player);

        public delegate bool WinConditionMethod(Scene scene);

        public event EventHandler<ForExternalUse.Synchronization.ISyncEventArgs> ReturnAction;

        public event EventHandler<ForExternalUse.Synchronization.IMoveEventArgs> ReturnMoveAction;

        private readonly object lockObject = new object();

        private readonly List<Player> players;

        private int updateCounter;

        public bool IsActive { get; private set; }

        public float PassedTime { get; private set; }

        public DefeatConditionMethod DefeatCondition { get; }

        public WinConditionMethod WinCondition { get; }

        public Random GameRandom { get; }

        public Tile[][] Tiles { get; private set; }

        public List<Actor> Actors { get; }

        public List<ActiveDecoration> Decorations { get; }

        public List<Actor> DeletedActors { get; private set; }

        public List<ActiveDecoration> DeletedDecorations { get; private set; }

        public int IdsCounter { get; private set; }

        public IVarManager VarManager { get; }

        public INativeManager NativeManager { get; }

        public int Version { get; private set; }

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
            int seed)
        {
            this.IsActive = true;
            this.Id = id;
            this.PassedTime = 0;
            this.GameRandom = new Random(seed);
            this.NativeManager = (INativeManager)nativeManager;
            this.VarManager = (IVarManager)varManager;
            ISceneGenerator tempGenerator = (ISceneGenerator)generator;
            this.WinCondition = tempGenerator.WinCondition;
            this.DefeatCondition = tempGenerator.DefeatCondition;
            this.IdsCounter = 1;
            this.players = new List<Player>();
            this.Actors = new List<Actor>();
            this.Decorations = new List<ActiveDecoration>();
            this.DeletedActors = new List<Actor>();
            this.DeletedDecorations = new List<ActiveDecoration>();
            updateCounter = 0;
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
            target.ChangeTempObject(actor);
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
            target.ChangeTempObject(actor);
            target.Native.OnCreateAction?.Invoke(this, target, actor);
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
            target.ChangeTempObject(decoration);
            target.Native.OnCreateAction?.Invoke(this, target, decoration);
            return decoration;
        }

        public Tile ChangeTile(string nativeName, int x, int y, float? height, Player owner)
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
                players,
                new List<Actor>(),
                new List<ActiveDecoration>(),
                new List<Actor>(),
                new List<ActiveDecoration>(),
                new Point(Tiles.Length, Tiles[0].Length),
                new List<Tile>(),
                RandomCounter);
        }

        private ForExternalUse.Synchronization.ISynchronizer GetSynchronizationData(bool nullify)
        {
            List<Actor> changedActors = Actors.FindAll(x => x.Affected);
            List<ActiveDecoration> changedDecorations = Decorations.FindAll(x => x.Affected);
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
                players,
                changedActors,
                changedDecorations,
                DeletedActors,
                DeletedDecorations,
                new Point(Tiles.Length, Tiles[0].Length),
                changedTiles,
                RandomCounter);
            if (nullify)
            {
                changedActors.ForEach(x => x.Affected = false);
                changedDecorations.ToList().ForEach(x => x.Affected = false);
                changedTiles.ToList().ForEach(x => x.Affected = false);
                this.DeletedDecorations = new List<ActiveDecoration>();
                this.DeletedActors = new List<Actor>();
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
                for (int x = 0; x < Tiles.Length; x++)
                {
                    for (int y = 0; y < Tiles[x].Length; y++)
                    {
                        Tiles[x][y].Affected = false;
                    }
                }

                this.DeletedDecorations.Clear();
                this.DeletedActors.Clear();
            }

            return new SynchronizerFull(players, Actors, Decorations, Tiles, RandomCounter);
        }

        public ForExternalUse.Synchronization.ISynchronizer GetFullSynchronizationData()
        {
            return GetFullSynchronizationData(false);
        }

        // Updates methods
        public void StartGame()
        {
            this.ReturnAction(this, new SyncEventArgs(this, ++Version, Helpers.SceneAction.StartGame, GetFullSynchronizationData(true), null, null, null, null));
        }

        public void EndTurn()
        {
            foreach (var actor in Actors)
            {
                actor.Update();
            }

            AfterUpdateSynchronization(SceneAction.EndTurn, null, null, null, null);
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
        }

        public void Update()
        {
            if (IsActive)
            {
                lock (lockObject)
                {
                    updateCounter++;
                    if (updateCounter >= 10)
                    {
                        EndTurn();
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
                AfterUpdateSynchronization(Helpers.SceneAction.Leave, null, null, null, null);
            }

            return success;
        }

        public void ActorMove(string playerId, int targetX, int targetY)
        {
            var player = Players.FirstOrDefault(x => x.Id == playerId);
            if (IsActive && player.PlayerActor.IsAlive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                lock (lockObject)
                {
                    if (player.PlayerActor.IsAlive && IsActive)
                    {
                        bool result = player.PlayerActor.Move(Tiles[targetX][targetY]);
                        if (result)
                        {
                            ReturnMoveAction?.Invoke(this, new MoveEventArgs(this, new[] { new MoveInfo(player.PlayerActor.Id, targetX, targetY) }));
                        }
                    }
                }
            }
        }

        public void ActorCast(string playerId, int skillId, int targetX, int targetY)
        {
            var player = Players.FirstOrDefault(x => x.Id == playerId);
            if (IsActive && player.PlayerActor.IsAlive && player.PlayerActor.Acted && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                lock (lockObject)
                {
                    if (IsActive && player.PlayerActor.IsAlive && player.PlayerActor.Acted)
                    {
                        bool result = player.PlayerActor.Cast(skillId, Tiles[targetX][targetY]);
                        if (result)
                        {
                            AfterUpdateSynchronization(Helpers.SceneAction.Cast, player.PlayerActor, skillId, targetX, targetY);
                        }
                        else
                        {
                            AfterUpdateSynchronization(Helpers.SceneAction.Unsuccess, player.PlayerActor, null, targetX, targetY);
                        }
                    }
                }
            }
        }

        public void ActorAttack(string playerId, int targetX, int targetY)
        {
            var player = Players.FirstOrDefault(x => x.Id == playerId);
            if (IsActive && player.PlayerActor.IsAlive && player.PlayerActor.Acted && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                lock (lockObject)
                {
                    if (IsActive && player.PlayerActor.IsAlive && player.PlayerActor.Acted)
                    {
                        bool result = player.PlayerActor.Attack(Tiles[targetX][targetY]);
                        if (result)
                        {
                            AfterUpdateSynchronization(Helpers.SceneAction.Attack, player.PlayerActor, null, targetX, targetY);
                        }
                        else
                        {
                            AfterUpdateSynchronization(Helpers.SceneAction.Unsuccess, player.PlayerActor, null, targetX, targetY);
                        }
                    }
                }
            }
        }

        public void ActorOrder(int actorId, int skillId, int targetX, int targetY)
        {
            if (IsActive && targetX >= 0 && targetY >= 0 && targetX < Tiles.Length && targetY < Tiles[0].Length)
            {
                var target = Tiles[targetX][targetY].TempObject;
                lock (lockObject)
                {
                    var actor = this.Actors.Find(x => x.Id == actorId);
                    if (IsActive && actor != null && (actor.Order == null || !actor.Order.Intended) && actor.Skills.Any(x => x.Id == skillId))
                    {
                        if (target != null && target.IsAlive)
                        {
                            actor.SetOrderModel(true, skillId, target);
                        }
                        else
                        {
                            actor.SetOrderModel(true, skillId, targetX, targetY);
                        }
                    }
                }
            }
        }
    }
}
