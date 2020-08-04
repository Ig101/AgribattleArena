import { Actor } from '../../scene/actor.object';
import { Observer } from 'rxjs';
import { ChangeDefinition } from './change-definition.model';

export interface Reaction {
  id: string;
  respondsOn: string;

  action: (actor: Actor, power: number, containerized: boolean,
           order: number, startingTime: number)
    => { power: number, changes: ChangeDefinition[] };
}
