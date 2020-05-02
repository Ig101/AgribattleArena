import { SyncTile } from 'src/app/shared/models/battle/synchronization/sync-tile.model';
import { tileNatives } from '../natives';
import { SyncSkill } from 'src/app/shared/models/battle/synchronization/sync-skill.model';
import { SyncBuff } from 'src/app/shared/models/battle/synchronization/sync-buff.model';
import { SyncActor } from 'src/app/shared/models/battle/synchronization/sync-actor.model';
import { Skill } from '../models/scene/skill.model';
import { Tile } from '../models/scene/tile.model';
import { Buff } from '../models/scene/buff.model';
import { Actor } from '../models/scene/actor.model';
import { Player } from '../models/player.model';
import { SyncDecoration } from 'src/app/shared/models/battle/synchronization/sync-decoration.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SyncSpecEffect } from 'src/app/shared/models/battle/synchronization/sync-spec-effect.model';
import { SpecEffect } from '../models/scene/spec-effect.model';
import { cloneActionAnimation, convertSkill, convertBuff } from './scene-create.helper';

export function synchronizeTile(tile: Tile, syncTile: SyncTile, owner?: Player) {
  if (syncTile.nativeId !== tile.nativeId) {
    const native = tileNatives[syncTile.nativeId];
    if (!native) {
      console.log(`Native error id: ${syncTile.nativeId}`);
    }
    tile.name = native.name;
    tile.description = native.description;
    tile.visualization = native.visualization;
    tile.backgroundColor = native.backgroundColor;
    tile.bright = native.bright;
    tile.action = cloneActionAnimation(native.action);
    tile.onStepAction = cloneActionAnimation(native.onStepAction);
    tile.nativeId = syncTile.nativeId;
    tile.unbearable = syncTile.unbearable;
  }
  if (owner) {
    tile.owner = owner;
  }
  tile.height = syncTile.height;
}

export function synchronizeSkill(skill: Skill, syncSkill: SyncSkill) {
  skill.cd = syncSkill.cd;
  skill.cost = syncSkill.cost;
  skill.mod = syncSkill.mod;
  skill.preparationTime = Math.ceil(syncSkill.preparationTime);
  skill.range = syncSkill.range;
  skill.meleeOnly = syncSkill.meleeOnly;
}

export function synchronizeBuff(buff: Buff, syncBuff: SyncBuff) {
  buff.mod = syncBuff.mod;
  buff.duration = syncBuff.duration;
}

function compareSkillIdArrays(array: Skill[], syncArray: SyncSkill[]) {
  if (syncArray.length !== array.length) {
    return false;
  }
  for (let i = 0; i < array.length; i++) {
    if (array[i].id !== syncArray[i].id ||
      array[i].preparationTime !== syncArray[i].preparationTime ||
      array[i].cost !== syncArray[i].cost) {
      return false;
    }
  }
  return true;
}

function compareBuffIdArrays(array: Buff[], syncArray: SyncBuff[]) {
  if (syncArray.length !== array.length) {
    return false;
  }
  for (let i = 0; i < array.length; i++) {
    if (array[i].id !== syncArray[i].id || array[i].duration !== syncArray[i].duration) {
      return false;
    }
  }
  return true;
}

export function synchronizeActor(actor: Actor, syncActor: SyncActor, isCurrentPlayerTeam: boolean, owner?: Player) {
  if (actor.attackingSkill?.id !== syncActor.attackingSkill?.id) {
    actor.attackingSkill = convertSkill(syncActor.attackingSkill, isCurrentPlayerTeam);
  } else if (actor.attackingSkill) {
    synchronizeSkill(actor.attackingSkill, syncActor.attackingSkill);
  }
  if (!compareSkillIdArrays(actor.skills, syncActor.skills)) {
    actor.skills = syncActor.skills.map(x => {
      const skill = actor.skills.find(s => s.id === x.id);
      if (skill) {
        synchronizeSkill(skill, x);
        return skill;
      } else {
        return convertSkill(x, isCurrentPlayerTeam);
      }
    });
  }
  actor.strength = syncActor.strength;
  actor.willpower = syncActor.willpower;
  actor.constitution = syncActor.constitution;
  actor.speed = syncActor.speed;
  actor.actionPointsIncome = syncActor.actionPointsIncome;
  if (!compareBuffIdArrays(actor.buffs, syncActor.buffs)) {
    actor.buffs = syncActor.buffs.map(x => {
      const buff = actor.buffs.find(s => s.id === x.id);
      if (buff) {
        synchronizeBuff(buff, x);
        return buff;
      } else {
        return convertBuff(x);
      }
    });
  }
  actor.initiativePosition = syncActor.initiativePosition;
  actor.health = syncActor.health;
  if (owner) {
    actor.owner = owner;
  }
  actor.isAlive = syncActor.isAlive;
  actor.x = syncActor.x;
  actor.y = syncActor.y;
  actor.z = syncActor.z;
  actor.maxHealth = syncActor.maxHealth;
  actor.actionPoints = syncActor.actionPoints;
  actor.skillPower = syncActor.skillPower;
  actor.attackPower = syncActor.attackPower;
  actor.initiative = syncActor.initiative;
  actor.armor = syncActor.armor;
  actor.attackModifiers = syncActor.attackModifiers;
  actor.canAct = syncActor.canAct;
  actor.canMove = syncActor.canMove;
}

export function synchronizeDecoration(decoration: ActiveDecoration, syncDecoration: SyncDecoration, owner?: Player) {
  if (owner) {
    decoration.owner = owner;
  }
  decoration.mod = syncDecoration.mod;
  decoration.initiativePosition = syncDecoration.initiativePosition;
  decoration.health = syncDecoration.health;
  decoration.isAlive = syncDecoration.isAlive;
  decoration.x = syncDecoration.x;
  decoration.y = syncDecoration.y;
  decoration.z = syncDecoration.z;
  decoration.maxHealth = syncDecoration.maxHealth;
  decoration.armor = syncDecoration.armor;
}

export function synchronizeEffect(effect: SpecEffect, syncEffect: SyncSpecEffect, owner?: Player) {
  if (owner) {
    effect.owner = owner;
  }
  effect.isAlive = syncEffect.isAlive;
  effect.x = syncEffect.x;
  effect.y = syncEffect.y;
  effect.z = syncEffect.z;
  effect.duration = syncEffect.duration;
  effect.mod = syncEffect.mod;
}
