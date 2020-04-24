using NUnit.Framework;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Tests.Engine
{
    public class DamageModelShould
        {
        private DamageModel _damageModel;

        [Test]
        [TestCase(TestName = "Default(Health)")]
        public void Health()
        {
            _damageModel = new DamageModel(500, null);
            Assert.AreEqual(_damageModel.Health, 500, "Model has 500 health");
            Assert.AreEqual(_damageModel.MaxHealth, 500, "Model has 500 max health");
        }

        [Test]
        [TestCase(499, false, 1f, TestName = "Default(NonLethalDamage)")]
        [TestCase(500, true, 0, TestName = "Default(LethalDamage)")]
        [TestCase(750, true, -250f, TestName = "Default(OverkillDamage)")]
        public void DefaultDamage(float amount, bool expectedState, float expectedHealth)
        {
            _damageModel = new DamageModel(500, null);
            bool isDead = _damageModel.Damage(amount, null);
            Assert.That(isDead, Is.EqualTo(expectedState), "Deal " + amount + " damage to 500 health Actor");
            Assert.That(_damageModel.Health, Is.EqualTo(expectedHealth), "Amount of health after dealing " + amount + " damage to 500 health Actor");
        }

        [Test]
        [TestCase(999, false, 0.5f, TestName = "HalfResist(NonLethalDamage)")]
        [TestCase(1000, true, 0, TestName = "HalfResist(LethalDamage)")]
        [TestCase(1500, true, -250f, TestName = "HalfResist(OverkillDamage)")]
        public void HalfResistDamage(float amount, bool expectedState, float expectedHealth)
        {
            _damageModel = new DamageModel(500, new TagSynergy[] { new TagSynergy("damage", 0.5f) });
            bool isDead = _damageModel.Damage(amount, new string[] { "damage" });

            Assert.That(isDead, Is.EqualTo(expectedState), "Deal " + amount + " damage to half-resist 500 health Actor");
            Assert.That(_damageModel.Health, Is.EqualTo(expectedHealth), "Amount of health after dealing " + amount + " damage to half-resist 500 health Actor");
        }

        [Test]
        [TestCase(100000, false, TestName = "Immune(Damage)")]
        [TestCase(-100000, false, TestName = "Immune(Heal)")]
        public void ImmuneDamage(float amount, bool expectedState)
        {
            _damageModel = new DamageModel(500, new TagSynergy[] { new TagSynergy("damage", 0) })
            {
                Health = 250f
            };
            bool isDead = _damageModel.Damage(amount, new string[] { "damage" });

            Assert.That(isDead, Is.EqualTo(expectedState), "Deal " + amount + " damage to immune 250/500 health Actor");
            Assert.That(_damageModel.Health, Is.EqualTo(250), "Amount of health after dealing " + amount + " damage to immune 250/500 health Actor");
        }

        [Test]
        [TestCase(TestName = "Injured(Health)")]
        public void InjuredHealth()
        {
            _damageModel = new DamageModel(500, null)
            {
                Health = 250
            };
            Assert.AreEqual(_damageModel.Health, 250, "Model has 250 health");
            Assert.AreEqual(_damageModel.MaxHealth, 500, "Model has 500 max health");
        }

        [Test]
        [TestCase(100, false, 350f, TestName = "Injured(Heal)")]
        [TestCase(1000, false, 500f, TestName = "Injured(Overheal)")]
        public void HealToInjured(float amount, bool expectedState, float expectedHealth)
        {
            _damageModel = new DamageModel(500, null)
            {
                Health = 250
            };
            bool isDead = _damageModel.Damage(-amount, null);

            Assert.That(isDead, Is.EqualTo(expectedState), "Heal 250 health Actor by " + amount);
            Assert.That(_damageModel.Health, Is.EqualTo(expectedHealth), "Amount of health after healing 250 health Actor by " + amount);
        }
    }
}
