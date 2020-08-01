import { Actor } from '../../scene/actor.object';
import { ActionDefinition } from '../action-definition.model';
import { Observable, Observer } from 'rxjs';
import { IActor } from '../../interfaces/actor.interface';

export interface Action {
  id: string;
  cooldown: number;
  remainedTime: number;
  untargeted: number;
  power: number;

  aiPriority: number;

  validateActionTargeted: (actor: Actor, power: number, x: number, y: number) => boolean;
  validateActionUntargeted: (actor: Actor, power: number) => boolean;
  validateActionOnObject: (actor: Actor, power: number, target: IActor) => boolean;

  actionTargeted: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, x: number, y: number) => void;
  actionUntargeted: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number) => void;
  actionOnObject: (definitionsSub: Observer<ActionDefinition>, actor: Actor, power: number, target: IActor) => void;
}
