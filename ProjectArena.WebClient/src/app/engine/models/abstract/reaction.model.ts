import { Actor } from '../../scene/actor.object';
import { Observer } from 'rxjs';
import { ActionDefinition } from '../action-definition.model';

export interface Reaction {
  id: string;
  respondsOn: string;

  action: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, containerized: boolean, order: number) => number;
}
