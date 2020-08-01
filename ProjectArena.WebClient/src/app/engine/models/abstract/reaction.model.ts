import { Effect } from '../../content/effects';
import { Priority } from '../../content/priorities';
import { Actor } from '../../scene/actor.object';
import { Observer } from 'rxjs';
import { ActionDefinition } from '../action-definition.model';

export interface Reaction {
  visualization: string;
  respondsOn: Effect;
  priority: Priority;

  action: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, order: number) => void;
}
