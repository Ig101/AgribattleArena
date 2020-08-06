import { ActorReference } from './actor-reference.model';
import { BuffSynchronization } from './buff-synchronization.model';

export interface ActorSynchronization {
  reference: ActorReference;
  parentId: number;
  x: number;
  y: number;
  durability: number;
  actors: ActorSynchronization[];
  buffs: BuffSynchronization[];
}
