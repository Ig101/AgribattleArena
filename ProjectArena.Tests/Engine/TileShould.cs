using AgribattleArena.Engine.ForExternalUse.Synchronization;
using AgribattleArena.Engine.Objects;
using AgribattleArena.Tests.Engine.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgribattleArena.Tests.Engine
{
    [TestFixture]
    class TileShould: BasicEngineTester
    {
        [SetUp]
        public void Prepare()
        {
            _syncMessages = new List<ISyncEventArgs>();
            _scene = SceneSamples.CreateSimpleScene(this.EventHandler,false);
            _scene.ChangeTile("test_tile_effect", 1, 2, null, _scene.Players.ToArray()[0]);
            _scene.ChangeTile("test_tile_effect", 17, 2, null, null);
            _syncMessages.Clear();
        }

        [Test]
        public void StartState()
        {
            Assert.That(_scene.Tiles[1][2].Owner, Is.EqualTo(_scene.Players.ToArray()[0]));
            Assert.That(_scene.Tiles[1][2].TempObject.DamageModel.Health, Is.EqualTo(90));
        }

        [Test]
        public void Impact()
        {
            _scene.ActorWait(_scene.TempTileObject.Id);
            Assert.That((int)_scene.Tiles[1][2].TempObject.DamageModel.Health, Is.EqualTo(85));
        }

        [Test]
        public void OnStepAction()
        {
            _scene.ActorMove(_scene.TempTileObject.Id, 17, 2);
            Assert.That(_scene.Tiles[17][2].TempObject.DamageModel.Health, Is.EqualTo(40));
        }
    }
}
