import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { Actor } from 'src/app/engine/scene/actor.object';
import { IActor } from 'src/app/engine/interfaces/actor.interface';
import { Tile } from 'src/app/engine/scene/tile.object';
import { moveToTarget, canStandOnTarget, LARGE_ACTOR_TRESHOLD_VOLUME } from '../content.helper';

export function moveActorToTile(actor: Actor, targetTile: Tile, startingTime: number) {
  const heightDifference = actor.z - targetTile.height;
  const parent = actor.parentActor;
  const oldActorIndex = parent.actors.indexOf(actor);
  targetTile.handleEffects(['pressure'], actor.volume, false, 1, startingTime);
  moveToTarget(actor, targetTile);
  for (let i = 0; i < parent.actors.length; i++) {
    if (i < oldActorIndex || !parent.isRoot) {
      parent.actors[i].handleEffects(['lighten'], actor.volume, false, 1, startingTime);
    } else if (actor.height > 120) {
      parent.actors[i].handleEffects(['fall'], (actor.height - 120), false, 1, startingTime);
    }
  }
  if (heightDifference > 120) {
    actor.handleEffects(['fall'], (heightDifference - 120), false, 1, startingTime);
  }
}

export function moveValidation(actor: Actor, x: number, y: number): string {
  if (!actor.parentActor.isRoot) {
    return 'Cannot move from another actor';
  }
  const targetTile = actor.parentScene.tiles[x][y];
  if (!canStandOnTarget(actor, targetTile)) {
    return 'Cannot stay on the target location';
  }
  if (targetTile.height >= actor.z + 120) {
    return 'Target location is too high';
  }
  return undefined;
}

export function moveAction(actor: Actor, power: number, x: number, y: number, startingTime: number): ChangeDefinition[] {
  const targetTile = actor.parentScene.tiles[x][y];
  actor.parentActor.handleEffects(['act', 'move-act'], 0, true, 1, startingTime);
  actor.handleEffects(['act', 'move-act'], 0, false, 1, startingTime);
  return [{
    time: startingTime,
    tileStubs: undefined,
    logs: undefined,
    action: () => {
      moveActorToTile(actor, targetTile, startingTime);
    }
  }];
}
