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
import { MELEE_RANGE, UNTARGETED_RANGE, RANGED_RANGE } from './content.helper';
import { ReactionSynchronization } from '../shared/models/synchronization/objects/reaction-synchronization.model';

@Injectable({
  providedIn: 'root'
})
export class NativesCollection implements INativesCollection {

  buildReaction(synchronization: ReactionSynchronization): Reaction {
    const native = reactionNatives[synchronization.id];
    return {
      id: synchronization.id,
      mod: synchronization.mod,
      native
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
      native,
      remainedTime: synchronization.remainedTime,
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
      addedPreparationReactions: synchronization.addedPreparationReactions.map(x => this.buildReaction(x)),
      addedActiveReactions: synchronization.addedActiveReactions.map(x => this.buildReaction(x)),
      addedClearReactions: synchronization.addedClearReactions.map(x => this.buildReaction(x)),
      changedDurability: synchronization.changedDurability,
      changedSpeed: synchronization.changedSpeed
    };
  }
}
