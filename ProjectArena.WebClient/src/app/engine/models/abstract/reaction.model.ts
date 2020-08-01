import { Effect } from '../../content/effects';
import { Actor } from '../../scene/actor.object';
import { Observer } from 'rxjs';
import { ActionDefinition } from '../action-definition.model';

export interface Reaction {
  visualization: string;
  respondsOn: Effect;

  action: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, containerized: boolean, order: number) => number;
}
