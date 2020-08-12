import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { Actor } from 'src/app/engine/scene/actor.object';
import { LARGE_ACTOR_TRESHOLD_VOLUME } from '../content.helper';
import { IActor } from 'src/app/engine/interfaces/actor.interface';

export function attackAction(actor: Actor, power: number, target: IActor, startingTime: number): ChangeDefinition[] {
  return [{
    time: startingTime,
    tileStubs: undefined,
    logs: undefined,
    action: () => {
      actor.handleEffects(['act'], 1, false, 1, startingTime);
      target.handleEffects(['physical-damage'], power, false, 1, startingTime);
    }
  }];
}
