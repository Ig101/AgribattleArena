using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class TileShould : BasicEngineTester
    {
        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            Scene.ChangeTile("test_tile_effect", 1, 2, null, Scene.Players.ToArray()[0]);
            Scene.ChangeTile("test_tile_effect", 17, 2, null, null);
            SyncMessages.Clear();
        }

        [Test]
        public void StartState()
        {
            Assert.That(Scene.Tiles[1][2].Owner, Is.EqualTo(Scene.Players.ToArray()[0]));
            Assert.That(Scene.Tiles[1][2].TempObject.DamageModel.Health, Is.EqualTo(90));
        }

        [Test]
        public void Impact()
        {
            Scene.ActorWait(Scene.TempTileObject.Id);
            Assert.That((int)Scene.Tiles[1][2].TempObject.DamageModel.Health, Is.EqualTo(85));
        }

        [Test]
        public void OnStepAction()
        {
            Scene.ActorMove(Scene.TempTileObject.Id, 17, 2);
            Assert.That(Scene.Tiles[17][2].TempObject.DamageModel.Health, Is.EqualTo(40));
        }
    }
}
