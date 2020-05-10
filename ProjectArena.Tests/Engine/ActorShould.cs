using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class ActorShould : BasicEngineTester
    {
        private Actor _actor;

        private void EndTurnAssertion(int id, bool nextIsNotSame)
        {
            Assert.That(SyncMessages[^1].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.EndTurn), "EndTurn message action");
            if (nextIsNotSame)
            {
                Assert.That(Scene.TempTileObject.Id, Is.Not.EqualTo(id), "Another actor is acting ");
                Assert.That(Scene.Actors.Find(x => x.Id == id).InitiativePosition, Is.GreaterThan(0), "Initiative of _actor");
                Assert.That(Scene.TempTileObject.InitiativePosition, Is.EqualTo(0), "Initiative of tempTileObject");
            }
            else
            {
                Assert.That(Scene.TempTileObject.Id, Is.EqualTo(id), "Another actor is acting ");
            }
        }

        private void MoveCloseToEnemy(int x)
        {
            int step = 0;
            if (SceneHelper.GetOrderByGuid(_actor.ExternalId) == 1)
            {
                do
                {
                    step++;
                    _actor.ActionPoints = 4;
                    Assert.That(Scene.ActorMove(_actor.Id, _actor.X + 1, 2), Is.True, "Move step " + step);
                }
                while (_actor.X < x);
            }
            else
            {
                do
                {
                    step++;
                    _actor.ActionPoints = 4;
                    Assert.That(Scene.ActorMove(_actor.Id, _actor.X - 1, 2), Is.True, "Move step " + step);
                }
                while (_actor.X > x);
            }

            Assert.That(_actor.X, Is.EqualTo(x), "Close to enemy X");
            Assert.That(_actor.Y, Is.EqualTo(2), "Close to enemy Y");
        }

        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            SyncMessages.Clear();
            _actor = (Actor)Scene.TempTileObject;
        }

        [Test]
        public void StartState()
        {
            Assert.That(SceneHelper.GetOrderByGuid(_actor.ExternalId), Is.EqualTo(2), "TempTileObject externalId");
            Assert.That(_actor.TempTile.X, Is.EqualTo(18), "X position");
            Assert.That(_actor.TempTile.Y, Is.EqualTo(2), "Y position");
            Assert.That(_actor.ActionPoints, Is.EqualTo(4), "Amount of actionPoints");
            Assert.That(_actor.InitiativePosition, Is.EqualTo(0), "Initiative position");
            Assert.That(_actor.Skills.Count, Is.EqualTo(2), "Skills count");
            Assert.That(_actor.DamageModel.Health, Is.EqualTo(50), "Health amount");
        }

        [Test]
        public void CreateNewActor()
        {
            Actor actor = Scene.CreateActor(null, "test_actor", "test_roleModel", Scene.Tiles[4][4], null);
            Assert.That(Scene.Actors.Count, Is.EqualTo(3), "Count of actors");
            Assert.That(actor.Strength, Is.EqualTo(10), "Strength");
            Assert.That(actor.Willpower, Is.EqualTo(10), "Willpower");
            Assert.That(actor.Speed, Is.EqualTo(10), "Speed");
            Assert.That(actor.Constitution, Is.EqualTo(10), "Constitution");
            Assert.That(actor.ActionPointsIncome, Is.EqualTo(4), "Action points");
            Assert.That(actor.AttackingSkill.Native.Id, Is.EqualTo("test_actor_attack"), "Attack");
            Assert.That(actor.Skills.Count, Is.EqualTo(1), "Skills count");
        }

        [Test]
        public void Resurrection()
        {
            Actor resurrectionActor = Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == 1);
            Assert.That(Scene.Tiles[1][2].TempObject, Is.EqualTo(resurrectionActor), "Position");
            resurrectionActor.Kill();
            Scene.ActorWait(_actor.Id);
            Assert.That(Scene.Actors.Count, Is.EqualTo(1), "Actors count");
            EndTurnAssertion(_actor.Id, false);
            Scene.ResurrectActor(resurrectionActor, Scene.Tiles[2][2], 30);
            Assert.That(Scene.Tiles[2][2].TempObject, Is.EqualTo(resurrectionActor), "Resurrection actor position");
            Assert.That(Scene.Tiles[1][2].TempObject, Is.Null, "Previous position");
            Assert.That(Scene.Actors.Count, Is.EqualTo(2), "Actors count");
            Assert.That(resurrectionActor.DamageModel.Health, Is.EqualTo(30), "Health after resurrection");
        }

        [Test]
        [TestCase(2, false, TestName = "SpendActionPoints(2 points, 4 temp)")]
        [TestCase(4, true, TestName = "SpendActionPoints(4 points, 4 temp)")]
        [TestCase(6, true, TestName = "SpendActionPoints(6 points, 4 temp)")]
        public void SpendActionPoints(int points, bool endTurn)
        {
            _actor.SpendActionPoints(points);
            Assert.That(_actor.CheckActionAvailability(), Is.EqualTo(!endTurn), "End turn condition");
            Assert.That(_actor.ActionPoints, Is.EqualTo(Math.Max(0, 4 - points)), "Amount of action points after spending");
        }

        // Moving tests
        [Test]
        [TestCase(17, 2, true, TestName = "MoveToPoint(One left)")]
        [TestCase(18, 3, true, TestName = "MoveToPoint(One down)")]
        [TestCase(17, 3, false, TestName = "MoveToPoint(One left and down)")]
        [TestCase(16, 2, false, TestName = "MoveToPoint(Two left)")]
        [TestCase(18, 4, false, TestName = "MoveToPoint(Two down)")]
        [TestCase(19, 2, false, TestName = "MoveToPoint(One to the wall)")]
        public void MoveToPoint(int targetX, int targetY, bool available)
        {
            int tempX = _actor.TempTile.X;
            int tempY = _actor.TempTile.Y;
            Assert.That(Scene.ActorMove(_actor.Id, targetX, targetY), Is.EqualTo(available), "Actor can move");
            Assert.That(SyncMessages.Count, Is.EqualTo(available ? 1 : 0), "SyncMessages count");
            if (available)
            {
                Assert.That(SyncMessages[0].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.Move), "Move action");
                Assert.That(SyncMessages[0].TargetX, Is.EqualTo(targetX), "Move targetX");
                Assert.That(SyncMessages[0].TargetY, Is.EqualTo(targetY), "Move targetY");
            }

            Assert.That(_actor.TempTile.X, Is.EqualTo(available ? targetX : tempX), "X position of Actor2");
            Assert.That(_actor.TempTile.Y, Is.EqualTo(available ? targetY : tempY), "Y position of Actor2");
            Assert.That(_actor.ActionPoints, Is.EqualTo(available ? 3 : 4), "Amount of actionPoints");
        }

        [Test]
        [TestCase(true, false, TestName = "MoveToPoint(0=>9=>18=>27=>36 height)")]
        [TestCase(true, true, TestName = "MoveToPoint(0=>-9=>-18=>-27=>-36 height)")]
        [TestCase(false, false, TestName = "MoveToPoint(0=>36 height)")]
        [TestCase(false, true, TestName = "MoveToPoint(0=>-36 height)")]
        public void MoveHeight(bool availability, bool down)
        {
            _actor.ActionPoints = 6;
            Scene.ActorMove(_actor.Id, 17, 2);
            if (down)
            {
                Scene.ActorMove(_actor.Id, 16, 2);
                Scene.ActorMove(_actor.Id, 15, 2);
            }

            SyncMessages.Clear();
            int tempActorPositionX = down ? 15 : 17;

            if (availability)
            {
                Assert.That(_actor.X, Is.EqualTo(tempActorPositionX), "Move targetX start position");
                Assert.That(_actor.Y, Is.EqualTo(2), "Move targetY start position");
                _actor.ActionPoints = 2;
                Assert.That(Scene.ActorMove(_actor.Id, tempActorPositionX, 3), Is.True, "1 step availability");
                Assert.That(_actor.X, Is.EqualTo(tempActorPositionX), "Move targetX 1 step");
                Assert.That(_actor.Y, Is.EqualTo(3), "Move targetY 1 step");
                Assert.That(Math.Abs(_actor.TempTile.Height), Is.EqualTo(9));

                _actor.ActionPoints = 2;
                Assert.That(Scene.ActorMove(_actor.Id, tempActorPositionX, 4), Is.True, "1 step availability");
                Assert.That(_actor.X, Is.EqualTo(tempActorPositionX), "Move targetX 2 step");
                Assert.That(_actor.Y, Is.EqualTo(4), "Move targetY 2 step");
                Assert.That(Math.Abs(_actor.TempTile.Height), Is.EqualTo(18));

                _actor.ActionPoints = 2;
                Assert.That(Scene.ActorMove(_actor.Id, tempActorPositionX - 1, 4), Is.True, "1 step availability");
                Assert.That(_actor.X, Is.EqualTo(tempActorPositionX - 1), "Move targetX 3 step");
                Assert.That(_actor.Y, Is.EqualTo(4), "Move targetY 3 step");
                Assert.That(Math.Abs(_actor.TempTile.Height), Is.EqualTo(27));
            }
            else
            {
                Scene.ActorMove(_actor.Id, tempActorPositionX - 1, 2);
                Assert.That(_actor.X, Is.EqualTo(tempActorPositionX - 1), "Move targetX start position");
                Assert.That(_actor.Y, Is.EqualTo(2), "Move targetY start position");
            }

            _actor.ActionPoints = 2;
            Assert.That(Scene.ActorMove(_actor.Id, tempActorPositionX - 1, 3), Is.EqualTo(availability), "last step availability");
            Assert.That(_actor.X, Is.EqualTo(tempActorPositionX - 1), "Move targetX last step");
            Assert.That(_actor.Y, Is.EqualTo(availability ? 3 : 2), "Move targetY last step");
            Assert.That(Math.Abs(_actor.TempTile.Height), Is.EqualTo(availability ? 36 : 0));
        }

        [Test]
        [TestCase(TestName = "MoveToPoint(TileObject)")]
        public void MoveToTileObject()
        {
            MoveCloseToEnemy(2);
            Assert.That(_actor.ActionPoints, Is.EqualTo(3), "After MoveCloseToEnemy actionPoints");
            Assert.That(Scene.ActorMove(_actor.Id, _actor.X - 1, 2), Is.False, "Move to TileObject");
        }

        [Test]
        [TestCase(TestName ="MoveToPoint(Move until turn ends)")]
        public void EndTurnByMove()
        {
            for (int i = 0; i < 4; i++)
            {
                Assert.That(Scene.ActorMove(_actor.Id, _actor.X - 1, _actor.Y), Is.True, "Step " + i);
            }

            Assert.That(_actor.ActionPoints, Is.EqualTo(0), "Action points after spending");
            Assert.That(SyncMessages.Count, Is.EqualTo(5), "SyncMessages count after action points spending");
            EndTurnAssertion(_actor.Id, true);
        }

        // Attacking tests
        [Test]
        [TestCase(2, 1, true, false, TestName = "ActorAttack(second player, reachable enemy)")]
        [TestCase(3, 1, false, false, TestName = "ActorAttack(second player, not-reachable enemy)")]
        [TestCase(17, 18, true, true, TestName = "ActorAttack(first player, reachable enemy 1 cell)")]
        [TestCase(14, 18, true, true, TestName = "ActorAttack(first player, reachable enemy 4 cells)")]
        [TestCase(13, 18, false, true, TestName = "ActorAttack(first player, not-reachable enemy 5 cells)")]
        public void ActorAttack(int position, int targetX, bool success, bool firstPlayer)
        {
            if (firstPlayer)
            {
                Scene.ActorWait(_actor.Id);
                _actor = (Actor)Scene.TempTileObject;
            }

            MoveCloseToEnemy(position);
            SyncMessages.Clear();
            Assert.That(Scene.Tiles[targetX][_actor.Y].TempObject, Is.Not.Null, "Target is existing");
            Assert.That(Scene.ActorAttack(_actor.Id, targetX, _actor.Y), Is.EqualTo(success), "Attack succession");
            Assert.That(SyncMessages.Count, Is.EqualTo(success ? 1 : 0), "Amount of sync messages");
            if (success)
            {
                Assert.That(SyncMessages[0].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.Attack), "Attack action");
                Assert.That(SyncMessages[0].SyncInfo.ChangedActors.Count(), Is.EqualTo(2), "Amount of changed actors");
            }

            Assert.That(_actor.ActionPoints, Is.EqualTo(success ? 2 : 3), "Amount of action points after attack");
            Assert.That(Scene.Tiles[targetX][_actor.Y].TempObject.DamageModel.Health, Is.EqualTo(success ? 25 : Scene.Tiles[targetX][_actor.Y].TempObject.DamageModel.MaxHealth), "Target's health");
        }

        [Test]
        [TestCase(1, TestName = "ActorAttack(one 75 health hit)")]
        [TestCase(2, TestName = "ActorAttack(two 75 health hit)")]
        [TestCase(3, TestName = "ActorAttack(three 75 health hit)")]
        public void ActorAttackKill(int numberOfAttacks)
        {
            MoveCloseToEnemy(2);
            int turnKilled = 1;
            _actor.SpendActionPoints(-1);
            SyncMessages.Clear();
            Actor targetActor = (Actor)Scene.Tiles[1][_actor.Y].TempObject;
            for (int i = 0; i < numberOfAttacks; i++)
            {
                Assert.That(Scene.ActorAttack(_actor.Id, 1, _actor.Y), Is.EqualTo(true), "Attack succession");
                Assert.That(SyncMessages.Count, Is.EqualTo(i + 1), "Amount of sync messages");
                Assert.That(targetActor.IsAlive, Is.EqualTo(i < turnKilled), "Is target alive");
                Assert.That(_actor.ActionPoints, Is.EqualTo(3 - i));
                Assert.That(SyncMessages[i].SyncInfo.DeletedActors.Count(), Is.EqualTo(turnKilled == i ? 1 : 0), "Amount of killed actors");
                if (i < turnKilled)
                {
                    Assert.That(targetActor.DamageModel.Health, Is.GreaterThan(0), "Amount of target health");
                    Assert.That(Scene.Actors.Count(), Is.EqualTo(2), "Amount of actors");
                }
                else
                {
                    Assert.That(targetActor.DamageModel.Health, Is.LessThanOrEqualTo(0), "Amount of target health");
                    Assert.That(Scene.Actors.Count(), Is.EqualTo(1), "Amount of actors");
                }
            }
        }

        [Test]
        [TestCase(TestName = "ActorAttack(Attack until end turn)")]
        public void ActorAttackEndTurn()
        {
            int endTurn = 3;
            for (int i = 0; i < 5; i++)
            {
                Assert.That(Scene.ActorAttack(_actor.Id, 17, _actor.Y), Is.EqualTo(i <= endTurn), "Attack succession");
                Assert.That(SyncMessages.Count, Is.EqualTo(Math.Min(endTurn, i) + (i < endTurn ? 1 : 2)), "Amount of sync messages");
                Assert.That(Scene.TempTileObject.Id, i < endTurn ? Is.EqualTo(_actor.Id) : Is.Not.EqualTo(_actor.Id), "Turn of which player");
                if (i <= endTurn)
                {
                    Assert.That(SyncMessages[i].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.Attack), "Attack action");
                }

                if (i == endTurn)
                {
                    EndTurnAssertion(_actor.Id, true);
                }
            }
        }

        // Casting tests
        [Test]
        [TestCase(2, true, TestName = "SkillCast(melee, success)")]
        [TestCase(3, false, TestName = "SkillCast(melee, fail)")]
        public void FirstSkillCast(int position, bool success)
        {
            MoveCloseToEnemy(position);
            SyncMessages.Clear();
            int skillId = _actor.Skills.Find(x => x.Native.Id == "test_actor_skill").Id;
            Actor targetActor = (Actor)Scene.Tiles[1][_actor.Y].TempObject;
            for (int i = 0; i < 4; i++)
            {
                Assert.That(Scene.Tiles[1][_actor.Y].TempObject, i <= 1 || !success ? Is.Not.Null : Is.Null, "Target is existing " + i);
                Assert.That(Scene.ActorCast(_actor.Id, skillId, 1, _actor.Y), Is.EqualTo(success), "Cast succession " + i);
                if (i <= 2 && success)
                {
                    if (i == 2)
                    {
                        EndTurnAssertion(_actor.Id, false);
                    }

                    Assert.That(SyncMessages.Count, Is.EqualTo(i < 2 ? i + 1 : 4), "Amount of sync messages " + i);
                    if (i <= 2)
                    {
                        Assert.That(SyncMessages[i].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.Cast), "Cast action " + i);
                        Assert.That(SyncMessages[i].SkillActionId, Is.EqualTo(skillId), "Skill id " + i);
                        Assert.That(SyncMessages[i].SyncInfo.ChangedActors.Count(), Is.EqualTo(i < 1 ? 2 : 1), "Amount of changed actors " + i);
                    }

                    Assert.That(_actor.ActionPoints, Is.EqualTo(i < 2 ? 2 - i : 4), "Amount of action points after attack " + i);
                    if (i <= 1)
                    {
                        Assert.That(targetActor.DamageModel.Health, Is.EqualTo(40 - (60 * i)), "Target's health " + i);
                    }
                }
            }
        }

        [Test]
        [TestCase(5, true, TestName = "SkillCast(ranged, success)")]
        [TestCase(6, false, TestName = "SkillCast(ranged, fail)")]
        public void SecondSkillCast(int position, bool success)
        {
            MoveCloseToEnemy(position);
            SyncMessages.Clear();
            Skill skill = _actor.Skills.Find(x => x.Native.Id == "test_actor_skill_range");
            int skillId = skill.Id;
            _actor.ActionPoints = 5;
            Actor targetActor = (Actor)Scene.Tiles[1][_actor.Y].TempObject;
            Assert.That(Scene.Tiles[1][_actor.Y].TempObject, Is.Not.Null, "Target is existing");
            Assert.That(Scene.ActorCast(_actor.Id, skillId, 1, _actor.Y), Is.EqualTo(success), "Cast succession");
            if (success)
            {
                Assert.That(Scene.Tiles[1][_actor.Y].TempObject.DamageModel.Health, Is.EqualTo(80), "Amount of target health after first cast");
                Assert.That(_actor.ActionPoints, Is.EqualTo(3), "Action points after first cast");
                Assert.That(Scene.ActorCast(_actor.Id, skillId, 1, _actor.Y), Is.False, "Another cast succession");
                Assert.That(Scene.Tiles[1][_actor.Y].TempObject.DamageModel.Health, Is.EqualTo(80), "Amount of target health after second cast");
                Assert.That(_actor.ActionPoints, Is.EqualTo(3), "Action points after second cast");
                int i = 0;
                float tempPreparationTime = skill.PreparationTime;
                while (skill.PreparationTime > 0 && i < 100)
                {
                    i++;
                    Scene.ActorWait(Scene.TempTileObject.Id);
                    Assert.That(tempPreparationTime, Is.GreaterThan(skill.PreparationTime), "Preparation time diminishing");
                    tempPreparationTime = skill.PreparationTime;
                }

                Assert.That(i > 98, Is.False, "Cycle error");
                Assert.That(_actor.ActionPoints, Is.EqualTo(8), "Action points after waiting");
                Assert.That(Scene.ActorCast(_actor.Id, skillId, 1, _actor.Y), Is.True, "Another cast succession");
                Assert.That(_actor.ActionPoints, Is.EqualTo(6), "Action points after last cast");
                Assert.That(Scene.Tiles[1][_actor.Y].TempObject.DamageModel.Health, Is.EqualTo(60), "Amount of target health after last cast");
            }
        }

        // Waiting tests
        [Test]
        [TestCase(0, TestName = "Wait(1 turn, 4 points)")]
        public void Wait(int points)
        {
            Scene.ActorWait(_actor.Id);
            Assert.That(_actor.ActionPoints, Is.EqualTo(4 - points), "Amount of action points after Wait");
            Assert.That(SyncMessages[0].Action, Is.EqualTo(ProjectArena.Engine.Helpers.Action.Wait), "Action of Wait message");
            Assert.That(SyncMessages[0].SyncInfo.ChangedActors.Count(), Is.EqualTo(0), "Count of changed actors");
            EndTurnAssertion(_actor.Id, true);
        }

        [Test]
        [TestCase(2, TestName = "Wait(2 turns passed)")]
        [TestCase(3, TestName = "Wait(3 turns passed)")]
        [TestCase(4, TestName = "Wait(4 turns passed)")]
        [TestCase(5, TestName = "Wait(5 turns passed)")]
        [TestCase(6, TestName = "Wait(6 turns passed)")]
        public void MultipleWait(int repetitions)
        {
            int[] expectedExternalIds = new int[] { 2, 1, 2, 2, 1, 2, 2 };
            int[] expectedActionPoints = new int[] { 0, 0 };
            for (int i = 0; i < repetitions; i++)
            {
                Actor expectedActor = Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == expectedExternalIds[i]);
                expectedActionPoints[expectedExternalIds[i] - 1] = Math.Min(expectedActionPoints[expectedExternalIds[i] - 1] + expectedActor.ActionPointsIncome, 8);
                Assert.That(SceneHelper.GetOrderByGuid(((Actor)Scene.TempTileObject).ExternalId), Is.EqualTo(expectedExternalIds[i]), "ExternalId of temp actor");
                Assert.That(((Actor)Scene.TempTileObject).ActionPoints, Is.EqualTo(expectedActionPoints[expectedExternalIds[i] - 1]), "Action points of temp actor");
                Scene.ActorWait(expectedActor.Id);
                EndTurnAssertion(expectedActor.Id, expectedExternalIds[i + 1] != expectedExternalIds[i]);
                SyncMessages.Clear();
            }
        }
    }
}
