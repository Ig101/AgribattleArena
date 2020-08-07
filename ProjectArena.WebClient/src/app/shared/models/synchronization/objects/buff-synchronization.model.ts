import { ActionSynchronization } from './action-synchronization.model';

export interface BuffSynchronization {
  id: string;

  duration: number;
  maxStacks: number;
  counter: number;

  changedDurability: number;
  changedSpeed: number;

  actions: ActionSynchronization[];
}
