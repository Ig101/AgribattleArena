import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { AnimationTile } from '../../models/animations/animation-tile.model';

export function explosionIssueDeclaration(targetX: number, targetY: number, color: Color) {
  const colors = new Array<Color>(4);
  for (let i = 0; i < 4; i++) {
    colors[i] = {r: (Math.random() + 9) / 10 * color.r, g: (Math.random() + 9) / 10 * color.g, b: (Math.random() + 9) / 10 * color.b, a: 1};
  }
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: false,
    animationTiles: [{x: targetX, y: targetY, char: '*', color: colors[0], unitColorMultiplier: 0,
      unitAlpha: false, ignoreHeight: false, overflowHealth: true, priority: 10, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  for (let i = 0; i < 4; i++) {
    let tiles = new Array<AnimationTile>(4);
    for (let t = 0; t < 4; t++) {
      tiles[t] = {
        x: targetX + (t === 0 ? 1 : t === 2 ? -1 : 0),
        y: targetY + (t === 1 ? 1 : t === 3 ? -1 : 0),
        unitColorMultiplier: 0,
        char: '*',
        color: colors[i + t >= 4 ? (i + t - 4) : (i + t)],
        overflowHealth: true,
        workingOnSpecEffects: true,
        unitAlpha: false,
        ignoreHeight: false,
        priority: 10
      };
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: tiles,
      specificAction: undefined
    });
    tiles = new Array<AnimationTile>(4);
    for (let t = 0; t < 4; t++) {
      tiles[t] = {
        x: targetX + (t === 0 || t === 1 ? 1 : -1),
        y: targetY + (t === 0 || t === 2 ? 1 : -1),
        unitColorMultiplier: 0,
        char: '*',
        workingOnSpecEffects: true,
        color: colors[i + t >= 4 ? (i + t - 4) : (i + t)],
        overflowHealth: true,
        unitAlpha: false,
        ignoreHeight: false,
        priority: 10
      };
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: tiles,
      specificAction: undefined
    });
  }
  const newTiles = new Array<AnimationTile>(9);
  for (let x = -1; x <= 1; x++) {
    for (let y = -1; y <= 1; y++) {
      newTiles[(x + 1) * 3 + y + 1] = {
        x: targetX + x,
        y: targetY + y,
        char: '*',
        unitColorMultiplier: 0,
        color: {r: (Math.random() + 9) / 10 * color.r, g: (Math.random() + 9) / 10 * color.g, b: (Math.random() + 9) / 10 * color.b, a: 1},
        workingOnSpecEffects: true,
        overflowHealth: true,
        unitAlpha: false,
        ignoreHeight: false,
        priority: 10
      };
    }
  }
  frames.push({
    updateSynchronizer: true,
    animationTiles: newTiles,
    specificAction: undefined
  });
  return frames;
}
