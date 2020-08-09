import { Actor } from 'src/app/engine/scene/actor.object';
import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { IActor } from 'src/app/engine/interfaces/actor.interface';

export interface ActionNative {
  name: string;
  description: string;
  timeCost: number;
  range: number;
  cooldown: number;
  power: number;
  aiPriority: number;

  validateActionTargeted: (actor: Actor, power: number, x: number, y: number) => boolean;
  validateActionOnObject: (actor: Actor, power: number, target: IActor) => boolean;

  actionTargeted: (actor: Actor, power: number, x: number, y: number, startingTime: number)
    => ChangeDefinition[];
  actionOnObject: (actor: Actor, power: number, target: IActor, startingTime: number)
    => ChangeDefinition[];
}
