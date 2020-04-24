using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial.Buffs;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class BuffManagerShould : BasicEngineTester
        {
        private Actor _actor;

        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            SyncMessages.Clear();
            _actor = (Actor)Scene.TempTileObject;
        }

        [Test]
        [TestCase(1, TestName = "AddBuff(Default)")]
        [TestCase(5, TestName = "AddBuff(5 Default)")]
        public void AddDefaultBuff(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                _actor.BuffManager.AddBuff("test_buff_default", (i + 1) * 2, null);
            }

            Assert.That(_actor.Buffs.Count, Is.EqualTo(1), "Count of Buffs");
            Assert.That(_actor.DamageModel.Health, Is.EqualTo(50 + (amount * 2)), "Start amount of health");
            Assert.That(_actor.Strength, Is.EqualTo(10 + (amount * 2)), "Amount of strength");
            Scene.ActorWait(_actor.Id);
            Assert.That(_actor.Buffs.Count, Is.EqualTo(1), "Count of Buff after waiting");
            Assert.That(_actor.Buffs[0].Duration, Is.Null, "Buff duration after waiting");
            Assert.That(_actor.AttackModifiers.Count, Is.EqualTo(1), "Attacker count");
            Assert.That(_actor.AttackModifiers[0].SelfTag, Is.EqualTo("test_self_tag"), "Attacker self tag");
            Assert.That(_actor.Armor.Count, Is.EqualTo(2), "Armor count");
            Assert.That(_actor.Armor[1].TargetTag, Is.EqualTo("test_target_tag"), "Armor target tag");
        }

        [Test]
        [TestCase(TestName = "AddBuff(Can expire)")]
        public void AddDurationBuff()
        {
            _actor.BuffManager.AddBuff("test_buff_duration", 10, null);
            Buff testBuff = _actor.Buffs[0];
            Assert.That(testBuff.Duration, Is.Not.Null, "Duration is setted up");
            float tempDuration = (float)testBuff.Duration;
            int i = 0;
            while (testBuff.Duration > 0 && i < 100)
            {
                i++;
                Scene.ActorWait(Scene.TempTileObject.Id);
                Assert.That(tempDuration, Is.GreaterThan(testBuff.Duration), "Duration diminishing");
                tempDuration = (float)testBuff.Duration;
            }

            Assert.That(i > 98, Is.False, "Cycle error");
            Assert.That(_actor.Buffs.Count, Is.EqualTo(0), "Count of Buff after waiting");
            Assert.That(_actor.DamageModel.Health, Is.EqualTo(50), "Amount of health after end of buff");
        }

        [Test]
        [TestCase(TestName = "AddBuff(Debuff)")]
        public void AddDebuff()
        {
            _actor.BuffManager.AddBuff("test_debuff", 10, null);
            Assert.That(_actor.Buffs.Count, Is.EqualTo(1), "Count of Buffs");
            Assert.That(_actor.DamageModel.Health, Is.EqualTo(50), "Start amount of health");
            Scene.ActorWait(_actor.Id);
            Assert.That(_actor.DamageModel.Health, Is.LessThan(50), "Changed health after waiting");
        }

        [Test]
        [TestCase("test_buff_default", TestName = "AddBuff(5 Multiple)")]
        [TestCase("test_buff_multiple", TestName = "AddBuff(5 Multiple)")]
        [TestCase("test_buff_summarize", TestName = "AddBuff(5 Summarizing)")]
        public void AddMultipleBuffs(string name)
        {
            for (int i = 0; i < 5; i++)
            {
                _actor.BuffManager.AddBuff(name, 10, 1);
            }

            Assert.That(_actor.Buffs.Count, Is.EqualTo(name == "test_buff_multiple" ? 4 : 1), "Buffs count");
            Assert.That(_actor.Buffs[0].Duration, Is.EqualTo(name == "test_buff_summarize" ? 5 : 1), "Buffs duration");
            Assert.That(_actor.MaxHealth, Is.EqualTo(name == "test_buff_multiple" ? 90 : 60), "Actor health");
        }

        [Test]
        [TestCase(TestName = "Purge(After death)")]
        public void DeathWithEternal()
        {
            _actor.BuffManager.AddBuff("test_buff_multiple", 10, null);
            _actor.BuffManager.AddBuff("test_buff_multiple", 10, null);
            _actor.BuffManager.AddBuff("test_buff_eternal", 10, null);
            Assert.That(_actor.Buffs.Count, Is.EqualTo(3), "BuffsCount");
            Assert.That(_actor.MaxHealth, Is.EqualTo(80), "Actor health");
            _actor.Kill();
            Assert.That(_actor.MaxHealth, Is.EqualTo(60), "Actor health");
            Assert.That(_actor.Buffs.Count, Is.EqualTo(1), "BuffsCount");
        }

        [Test]
        [TestCase(TestName = "Purge(Default)")]
        public void PurgeWithEternal()
        {
            _actor.BuffManager.AddBuff("test_buff_default", 10, null);
            _actor.BuffManager.AddBuff("test_buff_eternal", 10, null);
            Assert.That(_actor.MaxHealth, Is.EqualTo(70), "Actor max health");
            Assert.That(_actor.Buffs.Count, Is.EqualTo(2), "BuffsCount");
            _actor.BuffManager.RemoveAllBuffs(false);
            Assert.That(_actor.MaxHealth, Is.EqualTo(60), "Actor max health");
            Assert.That((int)_actor.DamageModel.Health, Is.EqualTo(51), "Actor health");
            Assert.That(_actor.Buffs.Count, Is.EqualTo(1), "BuffsCount");
            Assert.That(_actor.AttackModifiers.Count, Is.EqualTo(0), "Amount off attack modifiers after purge");
            Assert.That(_actor.Armor.Count, Is.EqualTo(1), "Amount of armor after purge");
        }

        [Test]
        [TestCase(TestName = "Purge(OnlyDebuffs)")]
        public void PurgeDebuffsOnly()
        {
            _actor.BuffManager.AddBuff("test_buff_default", 10, null);
            _actor.BuffManager.AddBuff("test_buff_eternal", 10, null);
            _actor.BuffManager.AddBuff("test_debuff", 10, null);
            Assert.That(_actor.MaxHealth, Is.EqualTo(70), "Actor max health");
            Assert.That(_actor.Buffs.Count, Is.EqualTo(3), "BuffsCount");
            _actor.BuffManager.RemoveBuffsByTagsCondition(x => x.Contains("debuff"));
            Assert.That(_actor.MaxHealth, Is.EqualTo(70), "Actor max health");
            Assert.That((int)_actor.DamageModel.Health, Is.EqualTo(60), "Actor health");
            Assert.That(_actor.Buffs.Count, Is.EqualTo(2), "BuffsCount");
        }
    }
}
