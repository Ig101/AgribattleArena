import { ActorSynchronization } from './objects/actor-synchronization.model';
import { ActorReference } from './objects/actor-reference.model';

export interface Synchronizer {
  idCounterPosition: number;
  actors: ActorSynchronization[];
  removedActors: ActorReference[];
}
