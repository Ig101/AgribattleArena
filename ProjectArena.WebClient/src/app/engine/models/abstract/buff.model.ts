import { Actor } from '../../scene/actor.object';
import { Action } from './action.model';
import { Reaction } from './reaction.model';
export interface Buff {

  id: string;

  updatesOnTurnOnly: boolean;
  duration: number;
  maxStacks: number;
  counter: number;

  blockedActions: string[];
  blockedEffects: string[];
  blockAllActions: boolean;

  addedActions: Action[];
  addedPreparationReactions: Reaction[];
  addedActiveReactions: Reaction[];
  addedClearReactions: Reaction[];

  changedDurability: number;
  changedSpeed: number;
  parentActor: Actor;
}
