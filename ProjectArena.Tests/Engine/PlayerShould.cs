using AgribattleArena.Engine;
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
    class PlayerShould: BasicEngineTester
    {
        [SetUp]
        public void Prepare()
        {
            _syncMessages = new List<ISyncEventArgs>();
            _scene = SceneSamples.CreateSimpleScene(this.EventHandler, true);
            _scene.Actors.Find(x => x.ExternalId == 1).ChangePosition(_scene.Tiles[17][2], true);
            _syncMessages.Clear();
        }

        [Test]
        public void StartState()
        {
            Assert.That(_scene.Tiles[1][2].TempObject, Is.Null, "Previous actor position");
            Assert.That(((Actor)_scene.Tiles[17][2].TempObject).ExternalId, Is.EqualTo(1), "Previous actor position");
        }

        [Test]
        [TestCase(false, false, TestName = "PlayerVictory(Second)")]
        [TestCase(true, false, TestName = "PlayerVictory(First)")]
        [TestCase(false, true, TestName = "PlayerVictory(Spawn)")]
        public void PlayerVictory(bool first, bool spawn)
        {
            if (first) _scene.ActorWait(_scene.TempTileObject.Id);
            _syncMessages.Clear();
            int tileX = first ? 18 : 17;
            Actor deadMan = (Actor)_scene.Tiles[tileX][2].TempObject;
            if (spawn) _scene.CreateActor(_scene.Players.ToArray()[0], "test_actor", "test_roleModel", _scene.Tiles[4][4], null);
            _scene.ActorAttack(_scene.TempTileObject.Id, tileX, 2);
            _scene.ActorAttack(_scene.TempTileObject.Id, tileX, 2);
            Assert.That(!deadMan.IsAlive, Is.True, "Actor killed");
            Assert.That(_syncMessages.Count, Is.EqualTo(3), "Count of syncMessages");
            Assert.That(_syncMessages[2].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.EndGame));
            Assert.That(_scene.Players.ToArray()[first ? 0 : 1].Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Victorious));
            Assert.That(_scene.Players.ToArray()[first ? 1 : 0].Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Defeated));
            Assert.That(_scene.Actors.Count, Is.EqualTo(1), "Count of actors");
        }

        [Test]
        [TestCase(4, TestName = "SelfKillDefeat(Not end turn)")]
        [TestCase(1, TestName = "SelfKillDefeat(End turn)")]
        public void SelfKill(int actionPoints)
        {
            Actor actor = (Actor)_scene.TempTileObject;
            _scene.ActorAttack(actor.Id, 18, 2);
            Assert.That(actor.IsAlive, Is.False, "Actor is dead");
            Assert.That(_syncMessages.Count, Is.EqualTo(2), "SyncMessages count");
            Assert.That(_syncMessages[0].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.Attack));
            Assert.That(_syncMessages[1].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.EndGame));
            Assert.That(_scene.Players.ToArray()[0].Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Victorious));
            Assert.That(_scene.Players.ToArray()[1].Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Defeated));
        }

        [Test]
        [TestCase(1, TestName = "SkipTurn(1 turn)")]
        [TestCase(5, TestName = "SkipTurn(10 turns)")]
        public void SkipTurn (int amount)
        {
            Player tempPlayer = (Player)_scene.TempTileObject.Owner;
            int skippedTurns = 0;
            for (int t = 0; t < amount; t++)
            {
                if (_scene.TempTileObject.Owner != tempPlayer)
                {
                    _scene.ActorWait(_scene.TempTileObject.Id);
                    t--;
                }
                else if (tempPlayer.Status == AgribattleArena.Engine.Helpers.PlayerStatus.Playing)
                {
                    {
                        Assert.That(_scene.RemainedTurnTime, Is.EqualTo(t == 0 ? 80 : 20));
                        int i = 0;
                        while (tempPlayer.TurnsSkipped == skippedTurns && i < 100)
                        {
                            i++;
                            _scene.UpdateTime(10);
                        }
                        skippedTurns = tempPlayer.TurnsSkipped;
                        Assert.That(i > 98, Is.False, "Cycle error " + t);
                        Assert.That(_syncMessages.Count, Is.EqualTo(2), "Count of syncMessages "+ t);
                        Assert.That(_syncMessages[_syncMessages.Count - 2].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.SkipTurn), "SkipTurn message action "+t);
                        Assert.That(_syncMessages[_syncMessages.Count - 1].Action, 
                            tempPlayer.TurnsSkipped<3?Is.EqualTo(AgribattleArena.Engine.Helpers.Action.EndTurn):
                            Is.EqualTo(AgribattleArena.Engine.Helpers.Action.EndGame), "EndTurn message action "+ t);
                        if (tempPlayer.TurnsSkipped >= 3)
                            Assert.That(tempPlayer.Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Defeated), "Player status "+t);
                        else
                            Assert.That(tempPlayer.Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Playing), "Player status " + t);
                    }
                }
                else
                {
                    Assert.That(tempPlayer.Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Defeated), "Player status");
                    Assert.That(_scene.Actors.Count, Is.EqualTo(1), "Amount of actors after defeat");
                }
                _syncMessages.Clear();
            }
        }

        [Test]
        [TestCase(TestName = "SkipTurn (4 turns, Act on 2 turn)")]
        public void RefreshSkippedTurnsTime()
        {
            _scene.Actors.Find(x => x.ExternalId == 1).ChangePosition(_scene.Tiles[16][2], true);
            Player tempPlayer = (Player)_scene.TempTileObject.Owner;
            int skippedTurns = 0;
            for (int t = 0; t < 4; t++)
            {
                if (_scene.TempTileObject.Owner != tempPlayer)
                {
                    _scene.ActorWait(_scene.TempTileObject.Id);
                    t--;
                }
                else
                {
                    {
                        if (t == 2)
                        {
                            _scene.ActorMove(_scene.TempTileObject.Id,17,2);
                            Assert.That(_syncMessages.Count, Is.EqualTo(1), "Count of move syncMessages " + t);
                            Assert.That(tempPlayer.TurnsSkipped, Is.EqualTo(0), "Skipped turns after move");
                            _syncMessages.Clear();
                        }
                        Assert.That(_scene.RemainedTurnTime, Is.EqualTo(t == 0 || t==2 ? 80 : 20));
                        int i = 0;
                        skippedTurns = tempPlayer.TurnsSkipped;
                        while (tempPlayer.TurnsSkipped == skippedTurns && i < 100)
                        {
                            i++;
                            _scene.UpdateTime(10);
                        }
                        Assert.That(i > 98, Is.False, "Cycle error "+ t);
                        Assert.That(_syncMessages.Count, Is.EqualTo(2), "Count of syncMessages " + t);
                        Assert.That(_syncMessages[_syncMessages.Count - 2].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.SkipTurn), "SkipTurn message action "+ t);
                        Assert.That(_syncMessages[_syncMessages.Count - 1].Action, Is.EqualTo(AgribattleArena.Engine.Helpers.Action.EndTurn), "EndTurn message action "+ t);
                        Assert.That(tempPlayer.Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Playing), "Player status " + t);
                    }
                }
                Assert.That(tempPlayer.Status, Is.EqualTo(AgribattleArena.Engine.Helpers.PlayerStatus.Playing));
                _syncMessages.Clear();
            }
        }
    }
}
