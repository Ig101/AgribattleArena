import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { AnimationTile } from '../../models/animations/animation-tile.model';
import { rangeBetween, angleBetween } from 'src/app/helpers/math.helper';

export function chargeIssueDeclaration(issueX: number, issueY: number) {
  const frames: AnimationFrame[] = [];
  const newTiles = new Array<AnimationTile>();
  for (let i = 0; i < 3; i++) {
    for (let x = -1; x <= 1; x++) {
      for (let y = -1; y <= 1; y++) {
        if (x !== 0 || y !== 0) {
          newTiles.push({
            x: issueX + x,
            y: issueY + y,
            char: '*',
            unitColorMultiplier: 0,
            color: {
              r: (Math.random() + 9) / 10 * 180,
              g: (Math.random() + 9) / 10 * 180,
              b: (Math.random() + 9) / 10 * 180,
              a: 1 - i * 0.4},
            workingOnSpecEffects: true,
            overflowHealth: true,
            unitAlpha: false,
            ignoreHeight: false,
            priority: 10
          });
        }
      }
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: newTiles,
      specificAction: undefined
    });
  }
  return frames;
}

function getPosition(issueX: number, issueY: number, targetX: number, targetY: number) {
  const angle = angleBetween(targetX, targetY, issueX, issueY);
  const sin = Math.sin(angle) * 0.5;
  const cos = Math.cos(angle) * 0.5;
  let x = targetX;
  let y = targetY;
  while (Math.floor(x) === targetX && Math.floor(y) === targetY) {
      x += cos;
      y += sin;
  }
  return { x, y };
}

export function chargeSyncDeclaration(issueX: number, issueY: number, targetX: number, targetY: number, char: string, color: Color) {
  const frames: AnimationFrame[] = [];
  const position = getPosition(issueX, issueY, targetX, targetY);
  const range = rangeBetween(issueX, issueY, position.x, position.y);
  const angle = angleBetween(issueX, issueY, position.x, position.y);
  const sin = Math.sin(angle);
  const cos = Math.cos(angle);
  let x = issueX;
  let y = issueY;
  let incrementingRange = 0;
  const impactedTiles = [{ x: issueX, y: issueY }];
  while (incrementingRange <= range) {
    incrementingRange++;
    if (incrementingRange >= range) {
      x = position.x;
      y = position.y;
    } else {
      x = Math.floor(issueX + (incrementingRange * cos));
      y = Math.floor(issueY + (incrementingRange * sin));
    }
    if (!impactedTiles.some(k => k.x === x && k.y === y)) {
      impactedTiles.push({x, y});
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: impactedTiles.map(tile => {
        return {x: tile.x, y: tile.y, char, color, unitColorMultiplier: 0,
          unitAlpha: false, ignoreHeight: false, overflowHealth: true, priority: 10, workingOnSpecEffects: true};
      }),
      specificAction: undefined
    });
  }
  frames.push({
    updateSynchronizer: true,
    animationTiles: [{x: targetX, y: targetY, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}],
    specificAction: undefined
  });
  return frames;
}
