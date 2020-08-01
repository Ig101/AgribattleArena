import { Scene } from './scene.object';
import { IActor } from '../interfaces/actor.interface';
import { Player } from './abstract/player.object';
import { Action } from '../models/abstract/action.model';
import { Reaction } from '../models/abstract/reaction.model';
import { Effect } from '../content/effects';
import { ActionDefinition } from '../models/action-definition.model';
import { Buff } from './abstract/buff.object';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';

export class Actor implements IActor {

  id: number;

  actors: Actor[];
  parentActor: IActor;
  parentScene: Scene;
  owner: Player;

  selfMaxDurability: number;
  durability: number;

  selfTurnCost: number;
  initiativePosition: number;

  height: number;
  volume: number;
  freeVolume: number;
  isActive: boolean;

  isAlive = true;

  buffs: Buff[];
  selfActions: Action[];
  preparationReactions: Reaction[];
  activeReactions: Reaction[];
  clearReactions: Reaction[];

  maxDurability: number;
  turnCost: number;
  actions: Action[];
  blockedActions: Action[];
  blockedEffects: Effect[];

  get x() {
    return this.parentActor.x;
  }

  get y() {
    return this.parentActor.y;
  }

  get z() {
    return this.parentActor.getActorZ(this);
  }

  constructor() {

  }

  getActorZ(actor: Actor) {
    return 0;
  }

  validateTargeted(action: Action, x: number, y: number) {
    if (action.validateActionTargeted) {
      return action.validateActionTargeted(this, action.power, x, y);
    } else {
      return false;
    }
  }

  validateOnObject(action: Action, target: IActor) {
    if (action.validateActionOnObject) {
      action.validateActionOnObject(this, action.power, target);
    } else {
      return false;
    }
  }

  validateUntargeted(action: Action) {
    if (action.validateActionUntargeted) {
      action.validateActionUntargeted(this, action.power);
    } else {
      return false;
    }
  }


  actTargeted(action: Action, x: number, y: number) {
    if (action.actionTargeted) {
      action.actionTargeted(this.parentScene.definitionsSub, this, action.power, x, y);
    }
  }

  actOnObject(action: Action, target: IActor) {
    if (action.actionOnObject) {
      action.actionOnObject(this.parentScene.definitionsSub, this, action.power, target);
    }
  }

  actUntargeted(action: Action) {
    if (action.actionUntargeted) {
      action.actionUntargeted(this.parentScene.definitionsSub, this, action.power);
    }
  }

  processEffect(effect: Effect, power: number, order: number) {
    if (this.blockedEffects.includes(effect) || order > 100) {
      return;
    }

    if (this.preparationReactions) {
      for (const reaction of this.preparationReactions) {
        if (reaction.respondsOn === effect) {
          reaction.action(this.parentScene.definitionsSub, this, power, order + 1);
        }
      }
    }
    for (const buff of this.buffs) {
      if (buff.addedPreparationReactions) {
        for (const reaction of buff.addedPreparationReactions) {
          if (reaction.respondsOn === effect) {
            reaction.action(this.parentScene.definitionsSub, this, power, order + 1);
          }
        }
      }
    }

    if (this.activeReactions) {
      for (const reaction of this.activeReactions) {
        if (reaction.respondsOn === effect) {
          reaction.action(this.parentScene.definitionsSub, this, power, order + 1);
        }
      }
    }
    for (const buff of this.buffs) {
      if (buff.addedActiveReactions) {
        for (const reaction of buff.addedActiveReactions) {
          if (reaction.respondsOn === effect) {
            reaction.action(this.parentScene.definitionsSub, this, power, order + 1);
          }
        }
      }
    }

    if (this.clearReactions) {
      for (const reaction of this.clearReactions) {
        if (reaction.respondsOn === effect) {
          reaction.action(this.parentScene.definitionsSub, this, power, order + 1);
        }
      }
    }
    for (const buff of this.buffs) {
      if (buff.addedClearReactions) {
        for (const reaction of buff.addedClearReactions) {
          if (reaction.respondsOn === effect) {
            reaction.action(this.parentScene.definitionsSub, this, power, order + 1);
          }
        }
      }
    }
  }

  applyBuff(buff: Buff) {

  }

  purgeBuffs() {

  }

  updateBuffs() {

  }

  changeDurability(durability: number) {
    this.durability += durability;
    if (this.durability <= 0) {
      this.isAlive = false;
    }
  }

  addActor(actor: Actor) {
    this.actors.push(actor);
  }

  removeActor(actor: Actor) {
    removeFromArray(this.actors, actor);
  }

  move(x: number, y: number) {
    this.parentActor.removeActor(this);
    this.parentScene.tiles[x][y].addActor(this);
  }
}
