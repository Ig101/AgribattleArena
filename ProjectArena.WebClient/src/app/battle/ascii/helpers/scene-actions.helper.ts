import { Tile } from '../models/scene/tile.model';
import { rangeBetween, angleBetween } from 'src/app/helpers/math.helper';
import { Skill } from '../models/scene/skill.model';
import { Targets } from 'src/app/shared/models/battle/targets.model';
import { Actor } from '../models/scene/actor.model';

export function checkMilliness(initial: Tile, target: Tile, tiles: Tile[][]) {
    const range = rangeBetween(initial.x, initial.y, target.x, target.y);
    let incrementingRange = 0;
    const angle = angleBetween(initial.x, initial.y, target.x, target.y);
    const sin = Math.sin(angle);
    const cos = Math.cos(angle);
    let currentTile = initial;
    while (incrementingRange <= range) {
      incrementingRange++;
      let nextTarget: Tile;
      if (incrementingRange >= range) {
        nextTarget = target;
      } else {
        const nextX = Math.floor(initial.x + (incrementingRange * cos));
        const nextY = Math.floor(initial.y + (incrementingRange * sin));
        nextTarget = tiles[nextX][nextY];
      }

      if (nextTarget.height - currentTile.height >= 10 ||
        (nextTarget !== target && !nextTarget.decoration && !nextTarget.actor) ||
        nextTarget.unbearable) {

        return false;
      }

      currentTile = nextTarget;
    }

    return true;
}

export function checkSkillTargets(target: Tile, actor: Actor, targets: Targets): boolean {
  const allies =
    targets.allies &&
    (target.actor || target.decoration) &&
    target.actor !== actor &&
    (!target.actor || target.actor.owner?.team === actor.owner?.team) &&
    (!target.decoration || target.decoration.owner?.team === actor.owner?.team);
  const notAllies =
    targets.notAllies &&
    (target.actor || target.decoration) &&
    target.actor !== actor &&
    (!target.actor || !actor.owner?.team || target.actor.owner?.team !== actor.owner?.team) &&
    (!target.decoration || !actor.owner?.team || target.decoration.owner?.team !== actor.owner?.team);
  const self =
    targets.self &&
    target.actor === actor;
  const bearable =
    targets.bearable &&
    !target.actor &&
    !target.decoration &&
    !target.unbearable;
  const unbearable =
    targets.unbearable &&
    !target.actor &&
    !target.decoration &&
    target.unbearable;
  return allies || notAllies || self || bearable || unbearable;
}