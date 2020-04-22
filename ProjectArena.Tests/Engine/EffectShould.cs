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
    class EffectShould: BasicEngineTester
    {
        SpecEffect _effect;

        [SetUp]
        public void Prepare()
        {
            _syncMessages = new List<ISyncEventArgs>();
            _scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            _effect = _scene.CreateEffect(_scene.Players.First(), "test_effect", _scene.Tiles[1][2], null, 2, null);
            _syncMessages.Clear();
        }

        [Test]
        public void StateState()
        {
            Assert.That(_effect.Duration, Is.EqualTo(2), "Effect duration");
        }

        [Test]
        public void Impact()
        {
            _scene.ActorWait(_scene.TempTileObject.Id);
            Assert.That((int)_scene.Actors.Find(x => x.ExternalId == 1).DamageModel.Health, Is.EqualTo(95), "Actor health after impact");
            Assert.That(_effect.Duration, Is.LessThan(2), "Effect duration");
        }

        [Test]
        public void Death()
        {
            int i = 0;
            while(_effect.Duration>0 && i<100)
            {
                _scene.ActorWait(_scene.TempTileObject.Id);
            }
            Assert.That(i > 400, Is.False, "Cycle error");
            Assert.That((int)_scene.Actors.Find(x => x.ExternalId == 1).DamageModel.Health, Is.EqualTo(70), "Actor health after impact");
            Assert.That(_effect.Duration, Is.LessThanOrEqualTo(0), "Effect duration");
            Assert.That(_effect.IsAlive, Is.False, "Is alive");
            Assert.That(_syncMessages[_syncMessages.Count() - 1].SyncInfo.DeletedEffects.Count(), Is.EqualTo(1), "Count of deleted effects");
        }
    }
}
