import { shiftTimeByFrames } from 'src/app/engine/engine.helper';
import { Actor } from 'src/app/engine/scene/actor.object';

export function receiveDirectDamage(actor: Actor, power: number, mod: number, containerized: boolean, startingTime: number) {
  if (!containerized) {
    return {
      power,
      changes: [
        {
          time: startingTime,
          tileStubs: [
            {
              endTime: shiftTimeByFrames(startingTime, 1),
              actorLink: actor,
              active: true,
              color: { r: 255, g: 255, b: 255, a: 1 },
              priority: 1
            }
          ],
          logs: [],
          action: () => {
            actor.changeDurability(-power * mod);
          }
        }
      ]
    };
  }
  return { power, changes: [] };
}
