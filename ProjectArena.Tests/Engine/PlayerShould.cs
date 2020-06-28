using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Objects;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class PlayerShould : BasicEngineTester
    {
        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, true);
            Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == 1).ChangePosition(Scene.Tiles[17][2], true);
            SyncMessages.Clear();
        }

        [Test]
        public void StartState()
        {
            Assert.That(Scene.Tiles[1][2].TempObject, Is.Null, "Previous actor position");
            Assert.That(SceneHelper.GetOrderByGuid(((Actor)Scene.Tiles[17][2].TempObject).ExternalId), Is.EqualTo(1), "Previous actor position");
        }

        [Test]
        [TestCase(false, false, TestName = "PlayerVictory(Second)")]
        [TestCase(true, false, TestName = "PlayerVictory(First)")]
        [TestCase(false, true, TestName = "PlayerVictory(Spawn)")]
        public void PlayerVictory(bool first, bool spawn)
        {
            if (first)
            {
                Scene.ActorWait();
            }

            SyncMessages.Clear();
            int tileX = first ? 18 : 17;
            Actor deadMan = (Actor)Scene.Tiles[tileX][2].TempObject;
            if (spawn)
            {
                Scene.CreateActor(Scene.Players.ToArray()[0], "test_actor", "test_roleModel", Scene.Tiles[4][4], null, null, null);
            }

            Scene.ActorAttack(Scene.TempTileObject.Id, tileX, 2);
            Scene.ActorAttack(Scene.TempTileObject.Id, tileX, 2);
            Assert.That(!deadMan.IsAlive, Is.True, "Actor killed");
            Assert.That(SyncMessages.Count, Is.EqualTo(3), "Count of syncMessages");
            Assert.That(SyncMessages[2].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.EndGame));
            Assert.That(Scene.Players.ToArray()[first ? 0 : 1].Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Victorious));
            Assert.That(Scene.Players.ToArray()[first ? 1 : 0].Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Defeated));
            Assert.That(Scene.Actors.Count, Is.EqualTo(1), "Count of actors");
        }

        [Test]
        [TestCase(TestName = "SelfKillDefeat(Not end turn)")]
        [TestCase(TestName = "SelfKillDefeat(End turn)")]
        public void SelfKill()
        {
            Actor actor = (Actor)Scene.TempTileObject;
            Scene.ActorAttack(actor.Id, 18, 2);
            Assert.That(actor.IsAlive, Is.False, "Actor is dead");
            Assert.That(SyncMessages.Count, Is.EqualTo(2), "SyncMessages count");
            Assert.That(SyncMessages[0].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.Attack));
            Assert.That(SyncMessages[1].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.EndGame));
            Assert.That(Scene.Players.ToArray()[0].Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Victorious));
            Assert.That(Scene.Players.ToArray()[1].Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Defeated));
        }
    }
}
