import { Scene } from './scene.object';
import { IActor } from '../interfaces/actor.interface';
import { Player } from './abstract/player.object';
import { Action } from '../models/abstract/action.model';
import { Reaction } from '../models/abstract/reaction.model';
import { ActionDefinition } from '../models/action-definition.model';
import { Buff } from '../models/abstract/buff.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';

const maxTurnCost = 10;
const minTurnCost = 1;
const maxReactionsDepth = 100;

export class Actor implements IActor {

  id: number;

  tags: string[];

  changed: boolean;

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
  blockedEffects: string[];

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
    return action.actionUntargeted;
  }


  actTargeted(action: Action, x: number, y: number) {
    if (action.actionTargeted) {
      action.actionTargeted(this.parentScene.definitionsSub, this, action.power, x, y);
      action.remainedTime = action.cooldown;
    }
  }

  actOnObject(action: Action, target: IActor) {
    if (action.actionOnObject) {
      action.actionOnObject(this.parentScene.definitionsSub, this, action.power, target);
      action.remainedTime = action.cooldown;
    }
  }

  actUntargeted(action: Action) {
    if (action.actionUntargeted) {
      action.actionUntargeted(this.parentScene.definitionsSub, this, action.power);
      action.remainedTime = action.cooldown;
    }
  }

  handleEffects(effects: string[], power: number, containerized: boolean, order: number) {
    function processReactions(reactions: Reaction[], inlineEffects: string[], inlinePower: number): number {
      if (reactions) {
        for (const reaction of reactions) {
          if (inlineEffects.includes(reaction.respondsOn)) {
            inlinePower = reaction.action(this.parentScene.definitionsSub, this, inlinePower, containerized, order + 1);
          }
        }
      }
      return inlinePower;
    }

    if (!this.isAlive || order > maxReactionsDepth) {
      return;
    }
    const tempEffects = this.blockedEffects.length > 0 ? effects.filter(x => !this.blockedEffects.includes(x)) : effects;
    if (tempEffects.length === 0) {
      return;
    }

    let tempPower = power;

    tempPower = processReactions(this.preparationReactions, tempEffects, tempPower);
    for (const buff of this.buffs) {
      tempPower = processReactions(buff.addedPreparationReactions, tempEffects, tempPower);
    }

    tempPower = processReactions(this.activeReactions, tempEffects, tempPower);
    for (const buff of this.buffs) {
      tempPower = processReactions(buff.addedActiveReactions, tempEffects, tempPower);
    }

    tempPower = processReactions(this.clearReactions, tempEffects, tempPower);
    for (const buff of this.buffs) {
      tempPower = processReactions(buff.addedClearReactions, tempEffects, tempPower);
    }

    let resultPower = tempPower;
    for (const actor of this.actors) {
      const powerChange = actor.handleEffects(effects, tempPower, true, order + 1) - tempPower;
      resultPower += powerChange;
    }
    return resultPower;
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
    if (buff.blockAllActions) {
      this.actions.length = 0;
    } else {
      this.actions = [...this.actions, ...buff.addedActions].filter(x => this.blockedActions.includes(x.id));
    }
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
      let blockAllActions = false;
      for (const buff of this.buffs) {
        this.blockedEffects.push(...buff.blockedEffects);
        this.blockedActions.push(...buff.blockedActions);
        blockAllActions = blockAllActions || buff.blockAllActions;
        if (blockAllActions) {
          this.actions.length = 0;
        } else {
          this.actions = [...this.actions, ...buff.addedActions].filter(x => this.blockedActions.includes(x.id));
        }
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

  private isActorReadyToStartTurn() {
    return this.owner && this.initiativePosition <= 0 && this.actions.some(x => x.remainedTime <= 0);
  }

  startTurn(): Actor[] {
    this.initiativePosition--;
    for (const action of this.actions) {
      action.remainedTime--;
    }
    const result: Actor[] = this.isActorReadyToStartTurn() ? [this] : [];
    for (const actor of this.actors) {
      result.push(...actor.startTurn());
    }
    return result;
  }
}
