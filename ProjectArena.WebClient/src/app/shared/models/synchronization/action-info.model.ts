import { ActorReference } from './objects/actor-reference.model';
import { ActionType } from '../enum/action-type.enum';

export interface ActionInfo {
  actor: ActorReference;
  id: string;
  type: ActionType;
  x: number;
  y: number;
  targetId: number;
}
