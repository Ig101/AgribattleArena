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
                Scene.ActorWait(Scene.TempTileObject.Id);
            }

            SyncMessages.Clear();
            int tileX = first ? 18 : 17;
            Actor deadMan = (Actor)Scene.Tiles[tileX][2].TempObject;
            if (spawn)
            {
                Scene.CreateActor(Scene.Players.ToArray()[0], "test_actor", "test_roleModel", Scene.Tiles[4][4], null);
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

        [Test]
        [TestCase(1, TestName = "SkipTurn(1 turn)")]
        [TestCase(5, TestName = "SkipTurn(10 turns)")]
        public void SkipTurn(int amount)
        {
            Player tempPlayer = (Player)Scene.TempTileObject.Owner;
            int skippedTurns = 0;
            for (int t = 0; t < amount; t++)
            {
                if (Scene.TempTileObject.Owner != tempPlayer)
                {
                    Scene.ActorWait(Scene.TempTileObject.Id);
                    t--;
                }
                else if (tempPlayer.Status == ProjectArena.Engine.Helpers.PlayerStatus.Playing)
                {
                    {
                        Assert.That(Scene.RemainedTurnTime, Is.EqualTo(t == 0 ? 100 : 20));
                        int i = 0;
                        while (tempPlayer.TurnsSkipped == skippedTurns && i < 100)
                        {
                            i++;
                            Scene.UpdateTime(10);
                        }

                        skippedTurns = tempPlayer.TurnsSkipped;
                        Assert.That(i > 98, Is.False, "Cycle error " + t);
                        Assert.That(SyncMessages.Count, Is.EqualTo(2), "Count of syncMessages " + t);
                        Assert.That(SyncMessages[^2].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.SkipTurn), "SkipTurn message action " + t);
                        Assert.That(
                            SyncMessages[^1].Action,
                            tempPlayer.TurnsSkipped < 3 ? Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.EndTurn) :
                            Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.EndGame), "EndTurn message action " + t);
                        if (tempPlayer.TurnsSkipped >= 3)
                        {
                            Assert.That(tempPlayer.Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Left), "Player status " + t);
                        }
                        else
                        {
                            Assert.That(tempPlayer.Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Playing), "Player status " + t);
                        }
                    }
                }
                else
                {
                    Assert.That(tempPlayer.Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Left), "Player status");
                    Assert.That(Scene.Actors.Count, Is.EqualTo(1), "Amount of actors after defeat");
                }

                SyncMessages.Clear();
            }
        }

        [Test]
        [TestCase(TestName = "SkipTurn (4 turns, Act on 2 turn)")]
        public void RefreshSkippedTurnsTime()
        {
            Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == 1).ChangePosition(Scene.Tiles[16][2], true);
            Player tempPlayer = (Player)Scene.TempTileObject.Owner;
            int skippedTurns = 0;
            for (int t = 0; t < 4; t++)
            {
                if (Scene.TempTileObject.Owner != tempPlayer)
                {
                    Scene.ActorWait(Scene.TempTileObject.Id);
                    t--;
                }
                else
                {
                    {
                        if (t == 2)
                        {
                            Scene.ActorMove(Scene.TempTileObject.Id, 17, 2);
                            Assert.That(SyncMessages.Count, Is.EqualTo(1), "Count of move syncMessages " + t);
                            Assert.That(tempPlayer.TurnsSkipped, Is.EqualTo(0), "Skipped turns after move");
                            SyncMessages.Clear();
                        }

                        Assert.That(Scene.RemainedTurnTime, Is.EqualTo(t == 0 ? 100 : t == 2 ? 80 : 20));
                        int i = 0;
                        skippedTurns = tempPlayer.TurnsSkipped;
                        while (tempPlayer.TurnsSkipped == skippedTurns && i < 100)
                        {
                            i++;
                            Scene.UpdateTime(10);
                        }

                        Assert.That(i > 98, Is.False, "Cycle error " + t);
                        Assert.That(SyncMessages.Count, Is.EqualTo(2), "Count of syncMessages " + t);
                        Assert.That(SyncMessages[^2].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.SkipTurn), "SkipTurn message action " + t);
                        Assert.That(SyncMessages[^1].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.EndTurn), "EndTurn message action " + t);
                        Assert.That(tempPlayer.Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Playing), "Player status " + t);
                    }
                }

                Assert.That(tempPlayer.Status, Is.EqualTo(ProjectArena.Engine.Helpers.PlayerStatus.Playing));
                SyncMessages.Clear();
            }
        }
    }
}
