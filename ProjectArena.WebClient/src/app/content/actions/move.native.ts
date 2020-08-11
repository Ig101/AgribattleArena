import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';
import { Actor } from 'src/app/engine/scene/actor.object';
import { IActor } from 'src/app/engine/interfaces/actor.interface';
import { Tile } from 'src/app/engine/scene/tile.object';

export function moveActorToTile(actor: Actor, targetTile: Tile, startingTime: number) {
  const heightDifference = actor.z - targetTile.height;
  const parent = actor.parentActor;
  targetTile.handleEffects(['pressure'], actor.volume, false, 1, startingTime);
  if (actor.height > 120 && actor.parentActor.isRoot) {
    const actorIndex = targetTile.actors.indexOf(actor);
    for (let i = actorIndex + 1; i < targetTile.actors.length; i++) {
      actor.handleEffects(['fall'], (actor.height - 120), false, 1, startingTime);
    }
  }
  actor.move(targetTile);
  parent.handleEffects(['lighten'], actor.volume, false, 1, startingTime);
  if (heightDifference > 120) {
    actor.handleEffects(['fall'], (heightDifference - 120), false, 1, startingTime);
  }
}

export function moveValidation(actor: Actor, x: number, y: number): string {
  const targetTile = actor.parentScene.tiles[x][y];
  if (targetTile.height >= actor.z + 120) {
    return 'Target location is too high';
  }
  return undefined;
}

export function moveAction(actor: Actor, power: number, x: number, y: number, startingTime: number): ChangeDefinition[] {
  const targetTile = actor.parentScene.tiles[x][y];
  return [{
    time: startingTime,
    tileStubs: undefined,
    logs: undefined,
    action: () => {
      moveActorToTile(actor, targetTile, startingTime);
    }
  }];
}
