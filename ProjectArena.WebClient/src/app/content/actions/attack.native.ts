import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { Actor } from 'src/app/engine/scene/actor.object';
import { LARGE_ACTOR_TRESHOLD_VOLUME } from '../content.helper';

export function attackAction(actor: Actor, power: number, x: number, y: number, startingTime: number): ChangeDefinition[] {
  const targetTile = actor.parentScene.tiles[x][y];
  let target;
  for (let i = targetTile.actors.length - 1; i >= 0; i++) {
    if (targetTile.actors[i].volume >= LARGE_ACTOR_TRESHOLD_VOLUME) {
      target = targetTile.actors[i];
      break;
    }
  }
  return [{
    time: startingTime,
    tileStubs: undefined,
    logs: undefined,
    action: () => {
      target.handleEffects(['physical-damage'], power, false, 1, startingTime);
    }
  }];
}
