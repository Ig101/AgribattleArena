import { Actor } from '../../scene/actor.object';
import { ActionDefinition } from '../action-definition.model';
import { Observable, Observer } from 'rxjs';
import { IActor } from '../../interfaces/actor.interface';

export interface Action {
  visualization: string;
  cooldown: number;
  remainedTime: number;
  untargeted: number;
  power: number;

  actionTargeted: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, x: number, y: number) => void;
  actionUntargeted: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number) => void;
  actionOnObject: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, target: IActor) => void;
}
