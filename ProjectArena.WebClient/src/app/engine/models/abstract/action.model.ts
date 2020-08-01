import { Actor } from '../../scene/actor.object';
import { ActionDefinition } from '../action-definition.model';
import { Observable, Observer } from 'rxjs';

export interface Action {
  visualization: string;
  cooldown: number;
  remainedTime: number;
  untargeted: number;

  action: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, x?: number, y?: number, id?: number) => void;
}
