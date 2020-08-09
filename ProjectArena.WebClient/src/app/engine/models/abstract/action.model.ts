import { Actor } from '../../scene/actor.object';
import { Observable, Observer } from 'rxjs';
import { IActor } from '../../interfaces/actor.interface';
import { ChangeDefinition } from './change-definition.model';
import { ActionClassEnum } from '../enums/action-class.enum';

export interface Action {
  id: string;
  name: string;
  description: string;
  onlyVisible: boolean;
  timeCost: number;
  range: number;
  cooldown: number;
  remainedTime: number;
  power: number;
  actionClass: ActionClassEnum;

  aiPriority: number;

  validateActionTargeted: (actor: Actor, power: number, x: number, y: number) => boolean;
  validateActionOnObject: (actor: Actor, power: number, target: IActor) => boolean;

  actionTargeted: (actor: Actor, power: number, x: number, y: number, startingTime: number)
    => ChangeDefinition[];
  actionOnObject: (actor: Actor, power: number, target: IActor, startingTime: number)
    => ChangeDefinition[];
}
