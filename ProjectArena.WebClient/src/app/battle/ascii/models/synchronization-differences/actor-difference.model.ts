import { Buff } from '../scene/buff.model';
import { Actor } from '../scene/actor.model';

export interface ActorDifference {
  x: number;
  y: number;
  actor: Actor;
  healthChange: number;
  newBuffs: Buff[];
  removedBuffs: Buff[];
  endedTurn: boolean;
  changedPosition: boolean;
}
