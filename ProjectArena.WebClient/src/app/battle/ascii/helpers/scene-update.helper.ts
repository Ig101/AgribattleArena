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
import { ActorDifference } from '../models/synchronization-differences/actor-difference.model';
import { DecorationDifference } from '../models/synchronization-differences/decoration-difference.model';

export function synchronizeTile(tile: Tile, syncTile: SyncTile, owner?: Player) {
  if (syncTile.nativeId !== tile.nativeId) {
    let native = tileNatives[syncTile.nativeId];
    if (!native) {
      native = {
        name: 'Undefined',
        description: undefined,
        visualization: {
          char: '!',
          color: { r: 200, g: 200, b: 0, a: 1 }
        },
        backgroundColor: { r: 15, g: 15, b: 0 },
        bright: false,
        action: undefined,
        onActionEffectAnimation: undefined,
        onStepAction: undefined
      };
      console.error(`Tile native ${tile.nativeId} is not found.`);
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
  skill.preparationTime = Math.ceil(syncSkill.preparationTime);
  skill.range = syncSkill.range;
  skill.meleeOnly = syncSkill.meleeOnly;
}

export function synchronizeBuff(buff: Buff, syncBuff: SyncBuff) {
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

function getActorDifference(actor: Actor, syncActor: SyncActor, difference: ActorDifference): ActorDifference {
  if (!difference) {
    difference = {
      x: syncActor.x,
      y: syncActor.y,
      actor,
      healthChange: 0,
      newBuffs: [],
      removedBuffs: [],
      endedTurn: false,
      changedPosition: false
    };
  }
  return difference;
}

export function synchronizeActor(actor: Actor, syncActor: SyncActor, isCurrentPlayerTeam: boolean, owner?: Player): ActorDifference {
  let difference: ActorDifference;
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
  if (!compareBuffIdArrays(actor.buffs, syncActor.buffs)) {
    const deletedBuffs = actor.buffs.filter(x => !syncActor.buffs.some(b => b.id === x.id));
    if (deletedBuffs.length > 0) {
      difference = getActorDifference(actor, syncActor, difference);
      difference.removedBuffs.push(...deletedBuffs);
    }
    actor.buffs = syncActor.buffs.map(x => {
      let buff = actor.buffs.find(s => s.id === x.id);
      if (buff) {
        synchronizeBuff(buff, x);
        return buff;
      } else {
        buff = convertBuff(x);
        difference = getActorDifference(actor, syncActor, difference);
        difference.newBuffs.push(buff);
        return buff;
      }
    });
  }
  actor.initiativePosition = syncActor.initiativePosition;
  const iniHealth = Math.floor(actor.health);
  const newHealth = Math.floor(syncActor.health);
  if (newHealth !== iniHealth) {
    difference = getActorDifference(actor, syncActor, difference);
    difference.healthChange = newHealth - iniHealth;
  }
  actor.health = syncActor.health;
  if (owner) {
    actor.owner = owner;
  }
  if (actor.x !== syncActor.x || actor.y !== syncActor.y) {
    difference = getActorDifference(actor, syncActor, difference);
    difference.changedPosition = true;
  }
  actor.x = syncActor.x;
  actor.y = syncActor.y;
  actor.z = syncActor.z;
  actor.maxHealth = syncActor.maxHealth;
  actor.actionPoints = syncActor.actionPoints;
  actor.initiative = syncActor.initiative;
  actor.canAct = syncActor.canAct;
  actor.canMove = syncActor.canMove;
  return difference;
}

function getDecorationDifference(decoration: ActiveDecoration, syncDecoration: SyncDecoration,
                                 difference: DecorationDifference): DecorationDifference {
  if (!difference) {
    difference = {
      x: syncDecoration.x,
      y: syncDecoration.y,
      decoration,
      healthChange: 0,
      changedPosition: false
    };
  }
  return difference;
}

export function synchronizeDecoration(decoration: ActiveDecoration, syncDecoration: SyncDecoration, owner?: Player): DecorationDifference {
  let difference: DecorationDifference;
  if (owner) {
    decoration.owner = owner;
  }
  decoration.mod = syncDecoration.mod;
  decoration.initiativePosition = syncDecoration.initiativePosition;
  const iniHealth = Math.floor(decoration.health);
  const newHealth = Math.floor(syncDecoration.health);
  if (newHealth !== iniHealth) {
    difference = getDecorationDifference(decoration, syncDecoration, difference);
    difference.healthChange = newHealth - iniHealth;
  }
  decoration.health = syncDecoration.health;
  decoration.isAlive = syncDecoration.isAlive;
  if (decoration.x !== syncDecoration.x || decoration.y !== syncDecoration.y) {
    difference = getDecorationDifference(decoration, syncDecoration, difference);
    difference.changedPosition = true;
  }
  decoration.x = syncDecoration.x;
  decoration.y = syncDecoration.y;
  decoration.z = syncDecoration.z;
  decoration.maxHealth = syncDecoration.maxHealth;
  decoration.armor = syncDecoration.armor;
  return difference;
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
