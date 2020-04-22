using AgribattleArena.Engine;
using AgribattleArena.Engine.ForExternalUse;
using AgribattleArena.Engine.ForExternalUse.EngineHelper;
using AgribattleArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using AgribattleArena.Engine.ForExternalUse.Synchronization;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Objects;
using AgribattleArena.Engine.Synchronizers;
using AgribattleArena.Tests.Engine.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgribattleArena.Tests.Engine
{
    [TestFixture]
    public class SceneShould: BasicEngineTester
    {
        [SetUp]
        public void Prepare ()
        {
            _syncMessages = new List<ISyncEventArgs>();
        }

        [Test]
        public void CreateSimpleScene ()
        {
            _scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);

            Assert.That(_scene, Is.Not.Null, "Check scene object existence");
            Assert.That(_syncMessages.Count, Is.EqualTo(2), "Check messages count");
            Assert.That(_syncMessages[0].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.StartGame), "Check StartGame message action");
            Assert.That(_syncMessages[0].Version, Is.EqualTo(0), "Check version of StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.TempActor, Is.Null, "Check tempActor in StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.Players.Count(), Is.EqualTo(2), "Check players count in StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.ChangedActors.Count(), Is.EqualTo(2), "Check changedActors count in StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.ChangedDecorations.Count(), Is.EqualTo(0), "Check changedDecorations count in StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.ChangedEffects.Count(), Is.EqualTo(0), "Check changedEffects count in StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.ChangedTiles.Count(), Is.EqualTo(400), "Check changedTiles count in StartGame message");
            Assert.That(_syncMessages[0].SyncInfo.DeletedActors.ToList().Count, Is.EqualTo(0), "Check deletedActors count in StartGame message");
            Assert.That(_syncMessages[1].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.EndTurn), "Check EndTurn message action");
            Assert.That(_syncMessages[1].Version, Is.EqualTo(1), "Check version of EndTurn message");
            Assert.That(_syncMessages[1].SyncInfo.TempActor, Is.Not.Null, "Check tempActor in EndTurn message");
            Assert.That(_syncMessages[1].SyncInfo.ChangedActors.Count(), Is.EqualTo(0), "Check changedActors count in EndTurn message");
            Assert.That(_syncMessages[1].SyncInfo.ChangedTiles.Count(), Is.EqualTo(0), "Check changedTiles count in EndTurn message");
            Assert.That(_syncMessages[1].SyncInfo.DeletedActors.Count(), Is.EqualTo(0), "Check deletedActors count in EndTurn message");
        }

        [Test]
        public void CreateSynchronizer()
        {
            _scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            _scene.Actors.Find(x => x.ExternalId == 1).ChangePosition(_scene.Tiles[17][2], true);
            _scene.CreateEffect(_scene.Players.First(), "test_effect", _scene.Tiles[1][2], null, 2, null);
            _scene.CreateDecoration(_scene.Players.First(), "test_decoration", _scene.Tiles[4][4], null, null, null, null);
            ISynchronizer synchronizer = new Synchronizer(
                _scene.TempTileObject, _scene.Players.ToList(), new List<Actor>() { _scene.Actors[0] }, new List<ActiveDecoration>() { _scene.Decorations[0] },
                new List<SpecEffect>(), new List<Actor>() { _scene.Actors[1] }, new List<ActiveDecoration>(), new List<SpecEffect>() { _scene.SpecEffects[0] },
                new AgribattleArena.Engine.Helpers.Point(20, 20), new List<Tile>() { _scene.Tiles[4][4] }, _scene.RandomCounter);
            Assert.That(synchronizer.TempActor.Id, Is.EqualTo(_scene.TempTileObject.Id), "Temp tile actor");
            Assert.That(synchronizer.TempDecoration, Is.EqualTo(null), "No temp tile decoration");
            Assert.That(synchronizer.ChangedActors.ToArray()[0].Id, Is.EqualTo(_scene.Actors[0].Id), "Rigth actors");
            Assert.That(synchronizer.ChangedDecorations.ToArray()[0].Id, Is.EqualTo(_scene.Decorations[0].Id), "Rigth decorations");
            Assert.That(synchronizer.ChangedEffects.Count(), Is.EqualTo(0), "Rigth effects");
            Assert.That(synchronizer.DeletedActors.ToArray()[0].Id, Is.EqualTo(_scene.Actors[1].Id), "Rigth deleted actors");
            Assert.That(synchronizer.DeletedDecorations.Count(), Is.EqualTo(0), "Rigth deleted decorations");
            Assert.That(synchronizer.DeletedEffects.ToArray()[0].Id, Is.EqualTo(_scene.SpecEffects[0].Id), "Rigth deleted effects");
            Assert.That(synchronizer.ChangedTiles.ToArray()[0].X, Is.EqualTo(4), "Rigth tiles");
            Assert.That(synchronizer.ChangedTiles.ToArray()[0].Y, Is.EqualTo(4), "Rigth tiles");
        }
    }
}
