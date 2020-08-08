import { ActorReference } from './actor-reference.model';
import { BuffSynchronization } from './buff-synchronization.model';
import { ActionSynchronization } from './action-synchronization.model';
import { Color } from '../../color.model';

export interface ActorSynchronization {
  reference: ActorReference;

  char: string;
  color: Color;
  backgroundColor: Color;

  ownerId: string;

  tags: string[];

  parentId: number;
  x: number;
  y: number;
  durability: number;
  maxDurability: number;
  turnCost: number;
  initiativePosition: number;
  height: number;
  volume: number;
  freeVolume: number;
  preparationReactions: string[];
  activeReactions: string[];
  clearReactions: string[];
  actions: ActionSynchronization[];
  actors: ActorSynchronization[];
  buffs: BuffSynchronization[];
}
