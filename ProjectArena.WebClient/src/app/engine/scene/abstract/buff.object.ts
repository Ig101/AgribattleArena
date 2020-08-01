import { Actor } from '../actor.object';
import { Action } from '../../models/abstract/action.model';
import { Reaction } from '../../models/abstract/reaction.model';

export class Buff {

  id: number;

  visualization: string;
  duration: number;
  maxStacks: number;

  blockedActions: string[];
  blockedReactions: string[];

  addedActions: Action[];
  addedReactions: Reaction[];

  changedDurability: number;
  changedSpeed: number;
  parentActor: Actor;
}
