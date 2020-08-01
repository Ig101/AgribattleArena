import { Scene } from './scene.object';
import { IActor } from '../interfaces/actor.interface';
import { Player } from './abstract/player.object';
import { Action } from '../models/abstract/action.model';
import { Reaction } from '../models/abstract/reaction.model';
import { Effect } from '../content/effects';
import { ActionDefinition } from '../models/action-definition.model';
import { Buff } from '../models/abstract/buff.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';

const maxTurnCost = 10;
const minTurnCost = 1;

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
  blockedActions: string[];
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

    for (const actor of this.actors) {
      actor.processEffect(effect, power, order + 1);
    }
  }

  applyBuff(buff: Buff) {
    const sameBuffs = this.buffs.filter(x => x.id === buff.id);
    if (buff.maxStacks <= sameBuffs.length) {
      return;
    }
    this.buffs.push(buff);
    this.turnCost += buff.changedSpeed;
    if (this.turnCost > maxTurnCost) {
      buff.changedSpeed -= (maxTurnCost - this.turnCost);
      this.turnCost = maxTurnCost;
    }
    if (this.turnCost < minTurnCost) {
      buff.changedSpeed += (minTurnCost - this.turnCost);
      this.turnCost = minTurnCost;
    }

    const newDurability = this.maxDurability + buff.changedDurability;
    if (newDurability < 0) {
      this.isAlive = false;
    } else if (newDurability !== this.maxDurability) {
      const difference = this.durability / this.maxDurability;
      this.durability = difference * newDurability;
      this.maxDurability = newDurability;
    }

    this.blockedEffects.push(...buff.blockedEffects);
    this.blockedActions.push(...buff.blockedActions);
    this.actions = [...this.actions, ...buff.addedActions].filter(x => this.blockedActions.includes(x.id));
  }

  purgeBuffs() {
    this.buffs.length = 0;
    this.blockedEffects.length = 0;
    this.blockedActions.length = 0;
    this.actions = [...this.selfActions];
  }

  updateBuffs() {
    let buffRemoved = false;
    for (const buff of this.buffs) {
      if (buff.duration !== undefined) {
        buff.duration--;
      }
      if (buff.duration <= 0) {
        buffRemoved = true;
        this.turnCost -= buff.changedSpeed;
        if (this.turnCost > maxTurnCost) {
          buff.changedSpeed -= (maxTurnCost - this.turnCost);
          this.turnCost = maxTurnCost;
        }
        if (this.turnCost < minTurnCost) {
          buff.changedSpeed += (minTurnCost - this.turnCost);
          this.turnCost = minTurnCost;
        }
        const newDurability = this.maxDurability - buff.changedDurability;
        if (newDurability < 0) {
          this.isAlive = false;
        } else if (newDurability !== this.maxDurability) {
          const difference = this.durability / this.maxDurability;
          this.durability = difference * newDurability;
          this.maxDurability = newDurability;
        }
      }
    }
    if (buffRemoved) {
      this.buffs = this.buffs.filter(x => x.duration > 0);
      this.blockedEffects.length = 0;
      this.blockedActions.length = 0;
      this.actions = [...this.selfActions];
      for (const buff of this.buffs) {
        this.blockedEffects.push(...buff.blockedEffects);
        this.blockedActions.push(...buff.blockedActions);
        this.actions = [...this.actions, ...buff.addedActions].filter(x => this.blockedActions.includes(x.id));
      }
    }
  }

  changeDurability(durability: number) {
    this.durability += durability;
    if (this.durability <= 0) {
      this.isAlive = false;
    }
    if (this.durability > this.maxDurability) {
      this.durability = this.maxDurability;
    }
  }

  addActor(actor: Actor) {
    this.actors.push(actor);
  }

  removeActor(actor: Actor) {
    removeFromArray(this.actors, actor);
  }

  move(target: IActor) {
    this.parentActor.removeActor(this);
    target.addActor(this);
  }
}
