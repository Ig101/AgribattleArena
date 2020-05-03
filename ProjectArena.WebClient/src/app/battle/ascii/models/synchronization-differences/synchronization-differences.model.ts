import { ActorDifference } from './actor-difference.model';
import { DecorationDifference } from './decoration-difference.model';

export interface SynchronizationDifference {
  actors: ActorDifference[];
  decorations: DecorationDifference[];
}
