import { Actor } from 'src/app/engine/scene/actor.object';
import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { LARGE_ACTOR_TRESHOLD_VOLUME } from '../content.helper';
import { IActor } from 'src/app/engine/interfaces/actor.interface';

export function shotAction(actor: Actor, power: number, target: IActor, startingTime: number): ChangeDefinition[] {
  const newPower = actor.handleEffects(['act', 'physical-attack'], power, false, 1, startingTime);
  return [{
    time: startingTime,
    tileStubs: undefined,
    logs: undefined,
    action: () => {
      target.handleEffects(['physical-damage'], newPower, false, 1, startingTime);
    }
  }];
}
