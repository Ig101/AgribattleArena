import { Actor } from '../../scene/actor.object';
import { Observable, Observer } from 'rxjs';
import { IActor } from '../../interfaces/actor.interface';
import { ChangeDefinition } from './change-definition.model';

export interface Action {
  id: string;
  timeCost: number;
  cooldown: number;
  remainedTime: number;
  untargeted: number;
  power: number;

  aiPriority: number;

  validateActionTargeted: (actor: Actor, power: number, x: number, y: number) => boolean;
  validateActionOnObject: (actor: Actor, power: number, target: IActor) => boolean;

  actionTargeted: (actor: Actor, power: number, x: number, y: number, startingTime: number)
    => ChangeDefinition[];
  actionUntargeted: (actor: Actor, power: number, startingTime: number)
    => ChangeDefinition[];
  actionOnObject: (actor: Actor, power: number, target: IActor, startingTime: number)
    => ChangeDefinition[];
}
