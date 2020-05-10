using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Objects;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class EffectShould : BasicEngineTester
        {
        private SpecEffect _effect;

        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            _effect = Scene.CreateEffect(Scene.Players.First(), "test_effect", Scene.Tiles[1][2], null, 2, null);
            SyncMessages.Clear();
        }

        [Test]
        public void StateState()
        {
            Assert.That(_effect.Duration, Is.EqualTo(2), "Effect duration");
        }

        [Test]
        public void Impact()
        {
            Scene.ActorWait(Scene.TempTileObject.Id);
            Assert.That((int)Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == 1).DamageModel.Health, Is.EqualTo(95), "Actor health after impact");
            Assert.That(_effect.Duration, Is.LessThan(2), "Effect duration");
        }

        [Test]
        public void Death()
        {
            int i = 0;
            while (_effect.Duration > 0 && i < 100)
            {
                Scene.ActorWait(Scene.TempTileObject.Id);
            }

            Assert.That(i > 400, Is.False, "Cycle error");
            Assert.That((int)Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == 1).DamageModel.Health, Is.EqualTo(70), "Actor health after impact");
            Assert.That(_effect.Duration, Is.LessThanOrEqualTo(0), "Effect duration");
            Assert.That(_effect.IsAlive, Is.False, "Is alive");
            Assert.That(SyncMessages[SyncMessages.Count() - 1].SyncInfo.DeletedEffects.Count(), Is.EqualTo(1), "Count of deleted effects");
        }
    }
}
