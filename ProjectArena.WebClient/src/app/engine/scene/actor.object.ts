import { Scene } from './scene.object';
import { IActor } from '../interfaces/actor.interface';
import { Player } from './abstract/player.object';
import { Action } from '../models/abstract/action.model';
import { Reaction } from '../models/abstract/reaction.model';
import { Buff } from '../models/abstract/buff.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { Color } from 'src/app/shared/models/color.model';

const maxTurnCost = 10;
const minTurnCost = 1;
const maxReactionsDepth = 100;

export class Actor implements IActor {

  id: number;

  char: string;
  color: Color;
  backgroundColor: Color;

  isAlive: boolean;

  tags: string[];

  changed: boolean;
  changedGlobally: boolean;

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
    this.isAlive = true;
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
    if (this.isAlive && action.actionTargeted) {
      //  this.parentScene.actionsSub.next();
      action.remainedTime = action.cooldown;
      this.parentScene.pushChanges(
        action.actionTargeted(this, action.power, x, y, this.parentScene.timeLine));
    }
  }

  actOnObject(action: Action, target: IActor) {
    if (this.isAlive && action.actionOnObject) {
      //  this.parentScene.actionsSub.next();
      action.remainedTime = action.cooldown;
      this.parentScene.pushChanges(
        action.actionOnObject(this, action.power, target, this.parentScene.timeLine));
    }
  }

  actUntargeted(action: Action) {
    if (this.isAlive && action.actionUntargeted) {
      //  this.parentScene.actionsSub.next();
      action.remainedTime = action.cooldown;
      this.parentScene.pushChanges(
        action.actionUntargeted(this, action.power, this.parentScene.timeLine));
    }
  }

  private processReactions(reactions: Reaction[], inlineEffects: string[], inlinePower: number,
                           containerized: boolean, order: number, startingTime: number): number {
    if (reactions) {
      for (const reaction of reactions) {
        if (inlineEffects.includes(reaction.respondsOn)) {
          const reactionResult =
            reaction.action(this, inlinePower, containerized, order + 1, startingTime);
          inlinePower = reactionResult.power;
          this.parentScene.pushChanges(reactionResult.changes);
        }
      }
    }
    return inlinePower;
  }

  handleEffects(effects: string[], power: number, containerized: boolean, order: number, startingTime: number) {
    if (!this.isAlive || order > maxReactionsDepth) {
      return;
    }
    const tempEffects = this.blockedEffects.length > 0 ? effects.filter(x => !this.blockedEffects.includes(x)) : effects;
    if (tempEffects.length === 0) {
      return;
    }

    let tempPower = power;

    tempPower = this.processReactions(this.preparationReactions, tempEffects, tempPower, containerized, order, startingTime);
    for (const buff of this.buffs) {
      tempPower = this.processReactions(buff.addedPreparationReactions, tempEffects, tempPower, containerized, order, startingTime);
    }

    tempPower = this.processReactions(this.activeReactions, tempEffects, tempPower, containerized, order, startingTime);
    for (const buff of this.buffs) {
      tempPower = this.processReactions(buff.addedActiveReactions, tempEffects, tempPower, containerized, order, startingTime);
    }

    tempPower = this.processReactions(this.clearReactions, tempEffects, tempPower, containerized, order, startingTime);
    for (const buff of this.buffs) {
      tempPower = this.processReactions(buff.addedClearReactions, tempEffects, tempPower, containerized, order, startingTime);
    }

    let resultPower = tempPower;
    for (const actor of this.actors) {
      const powerChange = actor.handleEffects(effects, tempPower, true, order + 1, startingTime) - tempPower;
      resultPower += powerChange;
    }
    return resultPower;
  }

  private calculateBuffPower(buff: Buff) {
    return buff.counter * 1000 + (buff.duration !== undefined ? buff.duration : 999);
  }

  kill() {
    this.isAlive = false;
    this.parentActor.removeActor(this);
    this.parentScene.removedActors.push(this.id);
  }

  applyBuff(buff: Buff) {
    this.changed = true;
    this.buffs.push(buff);
    const sameBuffs = this.buffs.filter(x => x.id === buff.id);
    const haveExtraBuffs = buff.maxStacks < sameBuffs.length;
    if (haveExtraBuffs) {
      const extraBuff = this.buffs.sort((a, b) => this.calculateBuffPower(a) - this.calculateBuffPower(b))[0];
      removeFromArray(this.buffs, extraBuff);
      this.turnCost -= buff.changedSpeed;
      if (this.turnCost > maxTurnCost) {
        this.turnCost = maxTurnCost;
      }
      if (this.turnCost < minTurnCost) {
        this.turnCost = minTurnCost;
      }
      const oldDurability = this.maxDurability - buff.changedDurability;
      if (oldDurability < 0) {
        this.kill();
      } else if (oldDurability !== this.maxDurability) {
        const difference = this.durability / this.maxDurability;
        this.durability = difference * oldDurability;
        this.maxDurability = oldDurability;
      }
    }
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
      this.kill();
    } else if (newDurability !== this.maxDurability) {
      const difference = this.durability / this.maxDurability;
      this.durability = difference * newDurability;
      this.maxDurability = newDurability;
    }

    if (!haveExtraBuffs) {
      this.blockedEffects.push(...buff.blockedEffects);
      this.blockedActions.push(...buff.blockedActions);
      if (buff.blockAllActions) {
        this.actions.length = 0;
      } else {
        this.actions = [...this.actions, ...buff.addedActions].filter(x => this.blockedActions.includes(x.id));
      }
    }
  }

  purgeBuffs() {
    this.changed = true;
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
      if (buff.duration !== undefined && buff.duration <= 0) {
        buffRemoved = true;
        this.turnCost -= buff.changedSpeed;
        if (this.turnCost > maxTurnCost) {
          this.turnCost = maxTurnCost;
        }
        if (this.turnCost < minTurnCost) {
          this.turnCost = minTurnCost;
        }
        const newDurability = this.maxDurability - buff.changedDurability;
        if (newDurability < 0) {
          this.kill();
        } else if (newDurability !== this.maxDurability) {
          const difference = this.durability / this.maxDurability;
          this.durability = difference * newDurability;
          this.maxDurability = newDurability;
        }
      }
    }
    if (buffRemoved) {
      this.changed = true;
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
    this.changed = true;
    this.durability += durability;
    if (this.durability <= 0) {
      this.kill();
    }
    if (this.durability > this.maxDurability) {
      this.durability = this.maxDurability;
    }
  }

  addActor(actor: Actor) {
    this.changed = true;
    this.actors.push(actor);
  }

  removeActor(actor: Actor) {
    this.changed = true;
    removeFromArray(this.actors, actor);
  }

  move(target: IActor) {
    this.changed = true;
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
