import { ActionSynchronization } from './action-synchronization.model';
import { ReactionSynchronization } from './reaction-synchronization.model';

export interface BuffSynchronization {
  id: string;

  duration: number;
  maxStacks: number;
  counter: number;

  changedDurability: number;
  changedSpeed: number;

  actions: ActionSynchronization[];
  addedPreparationReactions: ReactionSynchronization[];
  addedActiveReactions: ReactionSynchronization[];
  addedClearReactions: ReactionSynchronization[];
}
