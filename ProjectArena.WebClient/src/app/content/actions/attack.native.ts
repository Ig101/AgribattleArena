import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { Actor } from 'src/app/engine/scene/actor.object';
import { LARGE_ACTOR_TRESHOLD_VOLUME } from '../content.helper';
import { IActor } from 'src/app/engine/interfaces/actor.interface';

export function attackAction(actor: Actor, power: number, target: IActor, startingTime: number): ChangeDefinition[] {
  actor.parentActor.handleEffects(['act', 'physical-act', 'weapon-act'], power, true, 1, startingTime);
  const newPower = actor.handleEffects(['act', 'physical-act', 'weapon-act'], power, false, 1, startingTime);
  return [{
    time: startingTime,
    tileStubs: undefined,
    logs: undefined,
    action: () => {
      target.handleEffects(['physical-damage'], newPower, false, 1, startingTime);
    }
  }];
}
