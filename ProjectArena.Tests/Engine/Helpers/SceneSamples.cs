using System;
using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.EngineHelper;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Tests.Engine.Helpers
{
    public static class SceneSamples
    {
        private static void DoSelfDamage(ISceneParentRef scene, ActiveDecoration activeDecoration)
        {
            activeDecoration.Damage(activeDecoration.Mod, activeDecoration.Native.Tags);
        }

        private static void DoDamageOnStep(ISceneParentRef parent, Tile tile)
        {
            DoDamage(parent, tile, 1);
        }

        private static void DoDamage(ISceneParentRef parent, Tile tile, float time)
        {
            if (time > 0)
            {
                if (tile.TempObject != null)
                {
                    tile.TempObject.Damage(tile.Native.DefaultMod * time, tile.Native.Tags);
                }
            }
        }

        private static void DoDamageTempTile(ISceneParentRef parent, SpecEffect effect, float time)
        {
            if (time > 0)
            {
                var target = parent.Tiles[effect.X][effect.Y].TempObject;
                if (target != null)
                {
                    target.Damage(effect.Mod * time, effect.Native.Tags);
                }
            }
        }

        private static void DoDamageTempTileDeath(ISceneParentRef parent, SpecEffect effect)
        {
            DoDamageTempTile(parent, effect, 1);
        }

        private static void DoDamageAttack(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, ProjectArena.Engine.Objects.Immaterial.Skill skill)
        {
            if (targetTile.TempObject != null)
            {
                float mod = skill.CalculateModAttackPower(targetTile.TempObject.Native.Tags);
                targetTile.TempObject.Damage(mod, skill.AggregatedTags);
            }
        }

        private static void DoDamageSkill(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, ProjectArena.Engine.Objects.Immaterial.Skill skill)
        {
            if (targetTile.TempObject != null)
            {
                float mod = skill.CalculateModSkillPower(targetTile.TempObject.Native.Tags);
                targetTile.TempObject.Damage(mod, skill.AggregatedTags);
            }
        }

        private static void AddMaxHealth(ProjectArena.Engine.Objects.Immaterial.Buffs.IBuffManagerParentRef manager, ProjectArena.Engine.Objects.Immaterial.Buffs.Buff buff)
        {
            manager.AdditionMaxHealth += buff.Mod;
        }

        private static void AddStats(ProjectArena.Engine.Objects.Immaterial.Buffs.IBuffManagerParentRef manager, ProjectArena.Engine.Objects.Immaterial.Buffs.Buff buff)
        {
            manager.AdditionMaxHealth += buff.Mod;
            manager.AdditionStrength += buff.Mod;
            manager.Attack.Add(new TagSynergy("test_self_tag", "test_target_tag", buff.Mod));
            manager.Armor.Add(new TagSynergy("test_target_tag", buff.Mod));
        }

        private static void DamageSelf(ISceneParentRef scene, IActorParentRef actor, ProjectArena.Engine.Objects.Immaterial.Buffs.Buff buff, float time)
        {
            if (time > 0)
            {
                actor.Damage(buff.Mod * time, buff.Native.Tags);
            }
        }

        private static void DamageSelfPurge(ISceneParentRef scene, IActorParentRef actor, ProjectArena.Engine.Objects.Immaterial.Buffs.Buff buff)
        {
            DamageSelf(scene, actor, buff, 1);
        }

        private static void AddNatives(INativeManager nativeManager)
        {
            nativeManager.AddTileNative("test_wall", new string[0], false, 100, true, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h1", new string[0], false, 9, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h2", new string[0], false, 18, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h3", new string[0], false, 27, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h4", new string[0], false, 36, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h5", new string[0], false, -9, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h6", new string[0], false, -18, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h7", new string[0], false, -27, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_h8", new string[0], false, -36, false, 1, true, null, null);
            nativeManager.AddTileNative("test_tile_effect", new string[0], false, 0, false, 10, true, DoDamage, DoDamageOnStep);
            nativeManager.AddTileNative("test_tile", new string[0], false, 0, false, 1, true, null, null);
            nativeManager.AddActorNative("test_actor", "test_actor", "test_actor", new string[] { "test_actor_tag" }, 0, new TagSynergy[] { new TagSynergy("test_skill_tag", 0.5f) });
            nativeManager.AddEffectNative("test_effect", "test_effect", new string[0], 0, null, 10, DoDamageTempTile, DoDamageTempTileDeath);
            nativeManager.AddDecorationNative("test_decoration", "test_decoration", new string[0], new TagSynergy[] { }, 100, 0, 10, DoSelfDamage, DoSelfDamage);
            nativeManager.AddSkillNative("test_actor_attack", "test_actor_attack", "test_actor_attack", new string[0], 1, 1, 0, 75, new Targets() { NotAllies = true, Allies = true, Bearable = true, Unbearable = true, Self = true }, false, DoDamageAttack);
            nativeManager.AddSkillNative("test_actor_attack_range", "test_actor_attack_range", "test_actor_attack_range", new string[0], 4, 1, 0, 12.5f, new Targets() { NotAllies = true, Allies = true, Bearable = true, Unbearable = true, Self = true }, false, DoDamageAttack);
            nativeManager.AddSkillNative("test_actor_skill", "test_actor_skill", "test_actor_skill", new string[] { "test_skill_tag" }, 1, 1, 0, 60, new Targets() { NotAllies = true, Allies = true, Bearable = true, Unbearable = true, Self = true }, false, DoDamageSkill);
            nativeManager.AddSkillNative("test_actor_skill_range", "test_actor_skill_range", "test_actor_skill_range", new string[0], 4, 2, 2, 10, new Targets() { NotAllies = true, Allies = true, Bearable = true, Unbearable = true, Self = true }, false, DoDamageSkill);
            nativeManager.AddBuffNative("test_buff_default", new string[] { "buff" }, false, 1, false, false, null, 1, null, AddStats, DamageSelfPurge);
            nativeManager.AddBuffNative("test_buff_duration", new string[] { "buff" }, false, 1, false, false, 1, 1, null, null, null);
            nativeManager.AddBuffNative("test_buff_eternal", new string[] { "item" }, true, 1, false, false, null, 1, null, AddMaxHealth, DamageSelfPurge);
            nativeManager.AddBuffNative("test_buff_multiple", new string[] { "buff" }, false, 4, false, false, null, 1, null, AddMaxHealth, null);
            nativeManager.AddBuffNative("test_buff_summarize", new string[] { "buff" }, false, 1, true, false, null, 1, null, AddMaxHealth, null);
            nativeManager.AddBuffNative("test_debuff", new string[] { "debuff" }, false, 1, false, false, null, 1, DamageSelf, null, DamageSelfPurge);
            nativeManager.AddRoleModelNative("test_roleModel", 10, 10, 10, 10, 4, "test_actor_attack", new string[] { "test_actor_skill" });
        }

        public static Scene CreateSimpleScene(EventHandler<ISyncEventArgs> eventHandler, bool victory)
        {
            INativeManager nativeManager = EngineHelper.CreateNativeManager();
            AddNatives(nativeManager);
            string[,] tileSet = new string[20, 20];
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (x == 17 && y == 3)
                    {
                        tileSet[x, y] = "test_tile_h1";
                    }
                    else if (x == 17 && y == 4)
                    {
                        tileSet[x, y] = "test_tile_h2";
                    }
                    else if (x == 16 && y == 4)
                    {
                        tileSet[x, y] = "test_tile_h3";
                    }
                    else if (x == 16 && y == 3)
                    {
                        tileSet[x, y] = "test_tile_h4";
                    }
                    else if (x == 15 && y == 3)
                    {
                        tileSet[x, y] = "test_tile_h5";
                    }
                    else if (x == 15 && y == 4)
                    {
                        tileSet[x, y] = "test_tile_h6";
                    }
                    else if (x == 14 && y == 4)
                    {
                        tileSet[x, y] = "test_tile_h7";
                    }
                    else if (x == 14 && y == 3)
                    {
                        tileSet[x, y] = "test_tile_h8";
                    }
                    else if (x == 0 || y == 0 || x == 19 || y == 19)
                    {
                        tileSet[x, y] = "test_wall";
                    }
                    else
                    {
                        tileSet[x, y] = "test_tile";
                    }
                }
            }

            IActor[] firstPlayerActors = new IActor[]
            {
                EngineHelper.CreateActorForGeneration(Guid.Parse("10505fc7-3cdf-4c2b-bf36-4c934673a080"), "test_actor", "test_actor_attack_range", 20, 10, 20, 10, new string[] { "test_actor_skill", "test_actor_skill_range" }, 5, new string[0])
            };
            IActor[] secondPlayerActors = new IActor[]
            {
                EngineHelper.CreateActorForGeneration(Guid.Parse("d5bd9080-cd18-475d-9a30-1a1361f99e7e"), "test_actor", "test_actor_attack", 10, 20, 10, 18, new string[] { "test_actor_skill", "test_actor_skill_range" }, 4, new string[0])
            };
            var scene = SceneHelper.CreateNewScene(nativeManager, tileSet, victory, firstPlayerActors, secondPlayerActors, eventHandler);
            scene.EndTurn();
            return scene;
        }
    }
}
