import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';

export function summonIssueAnimation(x: number, y: number, color: Color, increment: Color) {
  const frames: AnimationFrame[] = [];
  for (let i = 0; i < 15; i++) {
    const frameColor = { r: color.r + increment.r * i, g: color.g + increment.r * i, b: color.b + increment.r * i, a: 1};
    frames.push({
      updateSynchronizer: false,
      animationTiles: [
        {x: x - 1, y, char: '-', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
        {x: x + 1, y, char: '-', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
        {x, y: y + 1, char: '|', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
        {x, y: y - 1, char: '|', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
      specificAction: undefined
    });
  }
  return frames;
}

export function summonSyncDeclaration(targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: true,
    animationTiles: [
      {x: targetX, y: targetY, char: '*', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}
