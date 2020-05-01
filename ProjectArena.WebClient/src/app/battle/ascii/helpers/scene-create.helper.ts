import { SyncTile } from 'src/app/shared/models/battle/synchronization/sync-tile.model';
import { tileNatives, skillNatives, actorNatives, buffNatives, decorationNatives, effectNatives } from '../natives';
import { SyncSkill } from 'src/app/shared/models/battle/synchronization/sync-skill.model';
import { SyncBuff } from 'src/app/shared/models/battle/synchronization/sync-buff.model';
import { SyncActor } from 'src/app/shared/models/battle/synchronization/sync-actor.model';
import { Skill } from '../models/scene/skill.model';
import { Tile } from '../models/scene/tile.model';
import { Buff } from '../models/scene/buff.model';
import { Actor } from '../models/scene/actor.model';
import { ActionAnimation } from '../models/action-animation.model';
import { TwoPhaseActionAnimation } from '../models/two-phase-action-animation.model';
import { BuffAnimation } from '../models/buff-animation.model';
import { Player } from '../models/player.model';
import { SyncDecoration } from 'src/app/shared/models/battle/synchronization/sync-decoration.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SyncSpecEffect } from 'src/app/shared/models/battle/synchronization/sync-spec-effect.model';
import { SpecEffect } from '../models/scene/spec-effect.model';

export function cloneActionAnimation(animation: ActionAnimation): ActionAnimation {
  return {

  };
}

export function cloneTwoPhaseActionAnimation(animation: TwoPhaseActionAnimation): TwoPhaseActionAnimation {
  return {

  };
}

export function cloneBuffAnimation(animation: BuffAnimation): BuffAnimation {
  return {

  };
}

export function convertTile(tile: SyncTile, owner: Player): Tile {
  const native = tileNatives[tile.nativeId];
  if (!native) {
    console.log(`Native error id: ${tile.nativeId}`);
  }
  return {
    x: tile.x,
    y: tile.y,
    name: native.name,
    description: native.description,
    visualization: native.visualization,
    backgroundColor: native.backgroundColor,
    bright: native.bright,
    action: cloneActionAnimation(native.action),
    onStepAction: cloneActionAnimation(native.onStepAction),
    height: tile.height,
    nativeId: tile.nativeId,
    owner,
    actor: undefined,
    decoration: undefined,
    specEffects: [],
    unbearable: tile.unbearable
  };
}

export function convertSkill(skill: SyncSkill, isCurrentPlayerTeam: boolean): Skill {
  const skillNative = skillNatives[skill.nativeId];
  if (!skillNative) {
    console.log(`Native error id: ${skill.nativeId}`);
  }
  return {
    id: skill.id,
    name: !skillNative.alternativeForm || isCurrentPlayerTeam ?
      skillNative.name :
      skillNative.enemyName,
    description: skillNative.description,
    action: !skillNative.alternativeForm || isCurrentPlayerTeam ?
      cloneTwoPhaseActionAnimation(skillNative.enemyAction) :
      cloneTwoPhaseActionAnimation(skillNative.enemyAction),
    range: skill.range,
    cd: skill.cd,
    mod: skill.mod,
    cost: skill.cost,
    preparationTime: skill.preparationTime,
    meleeOnly: skill.meleeOnly
  };
}

export function convertBuff(buff: SyncBuff): Buff {
  const buffNative = buffNatives[buff.nativeId];
  if (!buffNative) {
    console.log(`Native error id: ${buff.nativeId}`);
  }
  return {
    id: buff.id,
    char: buffNative.char,
    color: buffNative.color,
    name: buffNative.name,
    description: buffNative.description,
    onApplyAnimation: cloneActionAnimation(buffNative.onApplyAnimation),
    effectAnimation: cloneActionAnimation(buffNative.effectAnimation),
    onPurgeAnimation: cloneActionAnimation(buffNative.onPurgeAnimation),
    passiveAnimation: cloneBuffAnimation(buffNative.passiveAnimation),
    mod: buff.mod,
    duration: buff.duration
  };
}

export function convertActor(actor: SyncActor, owner: Player, isCurrentPlayerTeam: boolean): Actor {
  const native = actorNatives[actor.nativeId];
  if (!native) {
    console.log(`Native error id: ${actor.nativeId}`);
  }
  const visualization = isCurrentPlayerTeam ? native.visualization : native.enemyVisualization;
  const moveAction = isCurrentPlayerTeam ? native.moveAction : native.enemyMoveAction;
  return {
    id: actor.id,
    name: isCurrentPlayerTeam ? native.name : native.enemyName,
    description: isCurrentPlayerTeam ? native.description : native.enemyDescription,
    defaultVisualization: visualization,
    defaultMoveAction: moveAction,
    visualization: {
      char: visualization.char,
      color: { r: visualization.color.r, g: visualization.color.g, b: visualization.color. b, a: visualization.color.a }
    },
    moveAction: cloneTwoPhaseActionAnimation(moveAction),
    externalId: actor.externalId,
    attackingSkill: convertSkill(actor.attackingSkill, isCurrentPlayerTeam),
    strength: actor.strength,
    willpower: actor.willpower,
    constitution: actor.constitution,
    speed: actor.speed,
    skills: actor.skills.map(skill => convertSkill(skill, isCurrentPlayerTeam) ),
    actionPointsIncome: actor.actionPointsIncome,
    buffs: actor.buffs.map(buff => convertBuff(buff)),
    initiativePosition: actor.initiativePosition,
    health: actor.health,
    owner,
    isAlive: actor.isAlive,
    x: actor.x,
    y: actor.y,
    z: actor.z,
    maxHealth: actor.maxHealth,
    actionPoints: actor.actionPoints,
    skillPower: actor.skillPower,
    attackPower: actor.attackPower,
    initiative: actor.initiative,
    armor: actor.armor,
    attackModifiers: actor.attackModifiers,
    canAct: actor.canAct,
    canMove: actor.canMove
  };
}

export function convertDecoration(decoration: SyncDecoration, owner: Player, isCurrentPlayerTeam: boolean): ActiveDecoration {
  const native = decorationNatives[decoration.nativeId];
  if (!native) {
    console.log(`Native error id: ${decoration.nativeId}`);
  }
  return {
    id: decoration.id,
    name: !native.alternativeForm || isCurrentPlayerTeam ? native.name : native.enemyName,
    description: !native.alternativeForm || isCurrentPlayerTeam ? native.description : native.enemyDescription,
    visualization: !native.alternativeForm || isCurrentPlayerTeam ? native.visualization : native.enemyVisualization,
    action: !native.alternativeForm || isCurrentPlayerTeam ?
      cloneActionAnimation(native.action) :
      cloneActionAnimation(native.enemyAction),
    onDeathAction: !native.alternativeForm || isCurrentPlayerTeam ?
      cloneActionAnimation(native.onDeathAction) :
      cloneActionAnimation(native.enemyOnDeathAction),
    mod: decoration.mod,
    initiativePosition: decoration.initiativePosition,
    health: decoration.health,
    owner,
    isAlive: decoration.isAlive,
    x: decoration.x,
    y: decoration.y,
    z: decoration.z,
    maxHealth: decoration.maxHealth,
    armor: decoration.armor
  };
}

export function convertEffect(effect: SyncSpecEffect, owner: Player, isCurrentPlayerTeam: boolean): SpecEffect {
  const native = effectNatives[effect.nativeId];
  if (!native) {
    console.log(`Native error id: ${effect.nativeId}`);
  }
  return {
    id: effect.id,
    name: !native.alternativeForm || isCurrentPlayerTeam ? native.name : native.enemyName,
    description: !native.alternativeForm || isCurrentPlayerTeam ? native.description : native.enemyDescription,
    visualization: !native.alternativeForm || isCurrentPlayerTeam ? native.visualization : native.enemyVisualization,
    action: !native.alternativeForm || isCurrentPlayerTeam ?
      cloneActionAnimation(native.action) :
      cloneActionAnimation(native.enemyAction),
    onDeathAction: !native.alternativeForm || isCurrentPlayerTeam ?
      cloneActionAnimation(native.onDeathAction) :
      cloneActionAnimation(native.enemyOnDeathAction),
    owner,
    isAlive: effect.isAlive,
    x: effect.x,
    y: effect.y,
    z: effect.z,
    duration: effect.duration,
    mod: effect.mod
  };
}
