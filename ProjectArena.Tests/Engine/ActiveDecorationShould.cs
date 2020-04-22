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
    class ActiveDecorationShould: BasicEngineTester
    {
        ActiveDecoration _decoration;

        [SetUp]
        public void Prepare()
        {
            _syncMessages = new List<ISyncEventArgs>();
            _scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            _scene.Actors.Find(x => x.ExternalId == 1).Kill();
            _scene.ActorWait(_scene.TempTileObject.Id);
            _decoration = _scene.CreateDecoration(_scene.Players.First(), "test_decoration", _scene.Tiles[4][4], null, null, null, null);
            _syncMessages.Clear();
        }

        [Test]
        public void StartState()
        {
            Assert.That(_scene.Tiles[4][4].TempObject, Is.EqualTo(_decoration), "Position of decoration");
            Assert.That(_decoration.TempTile, Is.EqualTo(_scene.Tiles[4][4]), "Position of decoration");
            Assert.That(_decoration.DamageModel.Health, Is.EqualTo(100), "Health");
        }

        [Test]
        public void DecorationCast()
        {
            Assert.That(_scene.ActorWait(_scene.TempTileObject.Id), Is.True, "Actor first turn");
            Assert.That(_syncMessages.Count, Is.EqualTo(2), "Amount of syncMessages first turn");
            Assert.That(_decoration.DamageModel.Health, Is.EqualTo(100), "Health first turn");
            Assert.That(_scene.ActorWait(_scene.TempTileObject.Id), Is.True, "Actor second turn");
            Assert.That(_syncMessages.Count, Is.EqualTo(6), "Amount of syncMessages second turn");
            Assert.That(_decoration.DamageModel.Health, Is.EqualTo(90), "Health second turn");
            Assert.That(_syncMessages[4].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.Decoration), "Decoration action");
            for (int i = 0; i < 6; i++)
            {
                Assert.That(_syncMessages[i].SyncInfo.ChangedDecorations.Count(), Is.EqualTo(i==4 || i==0?1:0), "Check message decorations " + i);
            }
            Assert.That(_scene.TempTileObject, Is.EqualTo(_scene.Actors[0]), "Temp tile object at last");
        }

        [Test]
        public void DecorationDeath()
        {
            int i = 0;
            while(_decoration.DamageModel.Health>0 && i<500)
            {
                i++;
                _scene.ActorWait(_scene.TempTileObject.Id);
            }
            Assert.That(i>400, Is.False, "Cycle error");
            Assert.That(_syncMessages[_syncMessages.Count-2].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.Decoration), "Decoration action");
            Assert.That(_syncMessages[_syncMessages.Count - 2].SyncInfo.DeletedDecorations.Count(), Is.EqualTo(1), "Decoration killed");
            Assert.That(_syncMessages[_syncMessages.Count - 2].SyncInfo.DeletedDecorations.ToArray()[0].Id, Is.EqualTo(_decoration.Id), "Decoration killed id");
        }
    }
}
