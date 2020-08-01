import { Actor } from '../actor.object';
import { Action } from '../../models/abstract/action.model';
import { Reaction } from '../../models/abstract/reaction.model';
import { Effect } from '../../content/effects';

export class Buff {

  id: number;

  visualization: string;
  duration: number;
  maxStacks: number;

  blockedActions: Action[];
  blockedEffects: Effect[];

  addedActions: Action[];
  addedPreparationReactions: Reaction[];
  addedActiveReactions: Reaction[];
  addedClearReactions: Reaction[];

  changedDurability: number;
  changedSpeed: number;
  parentActor: Actor;
}
