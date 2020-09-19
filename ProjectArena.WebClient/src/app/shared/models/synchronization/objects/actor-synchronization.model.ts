import { ActorReference } from './actor-reference.model';
import { BuffSynchronization } from './buff-synchronization.model';
import { ActionSynchronization } from './action-synchronization.model';
import { Color } from '../../color.model';
import { ReactionSynchronization } from './reaction-synchronization.model';

export interface ActorSynchronization {
  reference: ActorReference;

  position: number;

  name: string;
  char: string;
  color: Color;
  left: boolean;

  ownerId: string;

  tags: string[];

  parentId: number;
  durability: number;
  maxDurability: number;
  turnCost: number;
  initiativePosition: number;
  height: number;
  volume: number;
  freeVolume: number;
  preparationReactions: ReactionSynchronization[];
  activeReactions: ReactionSynchronization[];
  clearReactions: ReactionSynchronization[];
  actions: ActionSynchronization[];
  actors: ActorSynchronization[];
  buffs: BuffSynchronization[];
}
