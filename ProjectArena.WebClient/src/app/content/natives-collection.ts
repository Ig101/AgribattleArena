import { INativesCollection } from '../engine/interfaces/natives-collection.interface';
import { Reaction } from '../engine/models/abstract/reaction.model';
import { ActionSynchronization } from '../shared/models/synchronization/objects/action-synchronization.model';
import { Action } from '../engine/models/abstract/action.model';
import { BuffSynchronization } from '../shared/models/synchronization/objects/buff-synchronization.model';
import { Buff } from '../engine/models/abstract/buff.model';
import { reactionNatives } from './reactions/reaction.natives';
import { actionNatives } from './actions/action.natives';
import { buffNatives } from './buffs/buff.natives';
import { Injectable } from '@angular/core';
import { ActionClassEnum } from '../engine/models/enums/action-class.enum';

export const MELEE_RANGE = 1;
export const UNTARGETED_RANGE = 0;
export const RANGED_RANGE = 8;

@Injectable({
  providedIn: 'root'
})
export class NativesCollection implements INativesCollection {

  buildReaction(id: string): Reaction {
    const native = reactionNatives[id];
    return {
      id,
      respondsOn: native.respondsOn,
      action: native.action
    };
  }

  buildAction(synchronization: ActionSynchronization): Action {
    const native = actionNatives[synchronization.id];
    let range;
    switch (native.actionClass) {
      case ActionClassEnum.Attack:
      case ActionClassEnum.Move:
        range = MELEE_RANGE;
        break;
      case ActionClassEnum.Autocast:
        range = UNTARGETED_RANGE;
        break;
      default:
        range = native.untargeted ? UNTARGETED_RANGE : RANGED_RANGE;
        break;
    }
    return {
      id: synchronization.id,
      name: native.name,
      description: native.description,
      timeCost: native.timeCost,
      range,
      cooldown: native.cooldown,
      remainedTime: synchronization.remainedTime,
      power: native.power,
      actionClass: native.actionClass,
      aiPriority: native.aiPriority,
      validateActionTargeted: native.validateActionTargeted,
      validateActionOnObject: native.validateActionOnObject,
      actionTargeted: native.actionTargeted,
      actionOnObject: native.actionOnObject
    };
  }

  buildBuff(synchronization: BuffSynchronization): Buff {
    const native = buffNatives[synchronization.id];
    return {
      id: synchronization.id,
      name: native.name,
      description: native.description,
      updatesOnTurnOnly: native.updatesOnTurnOnly,
      duration: synchronization.duration,
      maxStacks: synchronization.maxStacks,
      counter: synchronization.counter,
      blockedActions: native.blockedActions,
      blockedEffects: native.blockedEffects,
      blockAllActions: native.blockAllActions,
      addedActions: synchronization.actions.map(x => this.buildAction(x)),
      addedPreparationReactions: native.addedPreparationReactions,
      addedActiveReactions: native.addedActiveReactions,
      addedClearReactions: native.addedClearReactions,
      changedDurability: synchronization.changedDurability,
      changedSpeed: synchronization.changedSpeed
    };
  }
}
