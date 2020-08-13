import { IActor } from '../engine/interfaces/actor.interface';
import { Actor } from '../engine/scene/actor.object';

export const DEFAULT_HEIGHT = 500;
export const MELEE_RANGE = 1;
export const UNTARGETED_RANGE = 0;
export const RANGED_RANGE = 4;
export const VISIBILITY_AMPLIFICATION = 10;
export const LARGE_ACTOR_TRESHOLD_VOLUME = 50;

export function canStandOnTarget(actor: Actor, target: IActor) {
  if (actor.volume < LARGE_ACTOR_TRESHOLD_VOLUME ||
    !target.isRoot ||
    target.actors.length === 0) {
    return true;
  }
  let topActor: Actor;
  for (let i = target.actors.length - 1; i >= 0; i++) {
    if (target.actors[i].volume >= LARGE_ACTOR_TRESHOLD_VOLUME) {
      topActor = target.actors[i];
      break;
    }
  }
  return !topActor || topActor.tags.some(x => x === 'tile' || x === 'flat');
}

export function moveToTarget(actor: Actor, target: IActor) {
  if (!target.isRoot ||
    target.actors.length === 0) {

    actor.move(target);
    return;
  }
  let topFlatActorIndex = 0;
  for (let i = target.actors.length - 1; i >= 0; i--) {
    if (target.actors[i].volume >= LARGE_ACTOR_TRESHOLD_VOLUME && target.actors[i].tags.some(x => x === 'tile' || x === 'flat')) {
      topFlatActorIndex = i;
      break;
    }
  }
  for (let i = topFlatActorIndex + 1; i < target.actors.length - 1; i++) {
    if (target.actors[i].volume >= LARGE_ACTOR_TRESHOLD_VOLUME) {
      break;
    }
    topFlatActorIndex = i;
  }
  actor.moveToIndex(target, topFlatActorIndex + 1);
}
