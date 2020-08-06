import { ActorReference } from './objects/actor-reference.model';

export interface StartTurnInfo {
  tempActor: ActorReference;
  time: number;
}
