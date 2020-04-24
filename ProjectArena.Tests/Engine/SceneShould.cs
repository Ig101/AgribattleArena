using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Synchronizers;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class SceneShould : BasicEngineTester
    {
        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
        }

        [Test]
        public void CreateSimpleScene()
        {
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);

            Assert.That(Scene, Is.Not.Null, "Check scene object existence");
            Assert.That(SyncMessages.Count, Is.EqualTo(2), "Check messages count");
            Assert.That(SyncMessages[0].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.StartGame), "Check StartGame message action");
            Assert.That(SyncMessages[0].Version, Is.EqualTo(0), "Check version of StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.TempActor, Is.Null, "Check tempActor in StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.Players.Count(), Is.EqualTo(2), "Check players count in StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.ChangedActors.Count(), Is.EqualTo(2), "Check changedActors count in StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.ChangedDecorations.Count(), Is.EqualTo(0), "Check changedDecorations count in StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.ChangedEffects.Count(), Is.EqualTo(0), "Check changedEffects count in StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.ChangedTiles.Count(), Is.EqualTo(400), "Check changedTiles count in StartGame message");
            Assert.That(SyncMessages[0].SyncInfo.DeletedActors.ToList().Count, Is.EqualTo(0), "Check deletedActors count in StartGame message");
            Assert.That(SyncMessages[1].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.EndTurn), "Check EndTurn message action");
            Assert.That(SyncMessages[1].Version, Is.EqualTo(1), "Check version of EndTurn message");
            Assert.That(SyncMessages[1].SyncInfo.TempActor, Is.Not.Null, "Check tempActor in EndTurn message");
            Assert.That(SyncMessages[1].SyncInfo.ChangedActors.Count(), Is.EqualTo(0), "Check changedActors count in EndTurn message");
            Assert.That(SyncMessages[1].SyncInfo.ChangedTiles.Count(), Is.EqualTo(0), "Check changedTiles count in EndTurn message");
            Assert.That(SyncMessages[1].SyncInfo.DeletedActors.Count(), Is.EqualTo(0), "Check deletedActors count in EndTurn message");
        }

        [Test]
        public void CreateSynchronizer()
        {
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            Scene.Actors.Find(x => x.ExternalId == 1).ChangePosition(Scene.Tiles[17][2], true);
            Scene.CreateEffect(Scene.Players.First(), "test_effect", Scene.Tiles[1][2], null, 2, null);
            Scene.CreateDecoration(Scene.Players.First(), "test_decoration", Scene.Tiles[4][4], null, null, null, null);
            ISynchronizer synchronizer = new Synchronizer(
                Scene.TempTileObject,
                Scene.Players.ToList(),
                new List<Actor>() { Scene.Actors[0] },
                new List<ActiveDecoration>() { Scene.Decorations[0] },
                new List<SpecEffect>(),
                new List<Actor>() { Scene.Actors[1] },
                new List<ActiveDecoration>(),
                new List<SpecEffect>() { Scene.SpecEffects[0] },
                new ProjectArena.Engine.Helpers.Point(20, 20),
                new List<Tile>() { Scene.Tiles[4][4] },
                Scene.RandomCounter);
            Assert.That(synchronizer.TempActor.Id, Is.EqualTo(Scene.TempTileObject.Id), "Temp tile actor");
            Assert.That(synchronizer.TempDecoration, Is.EqualTo(null), "No temp tile decoration");
            Assert.That(synchronizer.ChangedActors.ToArray()[0].Id, Is.EqualTo(Scene.Actors[0].Id), "Rigth actors");
            Assert.That(synchronizer.ChangedDecorations.ToArray()[0].Id, Is.EqualTo(Scene.Decorations[0].Id), "Rigth decorations");
            Assert.That(synchronizer.ChangedEffects.Count(), Is.EqualTo(0), "Rigth effects");
            Assert.That(synchronizer.DeletedActors.ToArray()[0].Id, Is.EqualTo(Scene.Actors[1].Id), "Rigth deleted actors");
            Assert.That(synchronizer.DeletedDecorations.Count(), Is.EqualTo(0), "Rigth deleted decorations");
            Assert.That(synchronizer.DeletedEffects.ToArray()[0].Id, Is.EqualTo(Scene.SpecEffects[0].Id), "Rigth deleted effects");
            Assert.That(synchronizer.ChangedTiles.ToArray()[0].X, Is.EqualTo(4), "Rigth tiles");
            Assert.That(synchronizer.ChangedTiles.ToArray()[0].Y, Is.EqualTo(4), "Rigth tiles");
        }
    }
}
