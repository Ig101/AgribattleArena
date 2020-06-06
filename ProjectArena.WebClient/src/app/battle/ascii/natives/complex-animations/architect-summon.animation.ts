import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { Color } from 'src/app/shared/models/color.model';

export function architectSummonIssueDeclaration(targetX: number, targetY: number, color: Color, increment: Color) {
  const frames: AnimationFrame[] = [];
  for (let i = 0; i < 7; i++) {
    const frameColor = { r: color.r + increment.r * i, g: color.g + increment.g * i, b: color.b + increment.b * i, a: 1};
    frames.push({
      updateSynchronizer: false,
      animationTiles: [
        {x: targetX - 1, y: targetY - 1, char: '\\', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
        {x: targetX + 1, y: targetY + 1, char: '\\', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
        {x: targetX - 1, y: targetY + 1, char: '/', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
        {x: targetX + 1, y: targetY - 1, char: '/', color: frameColor, unitAlpha: false,
        unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
      specificAction: undefined
    });
  }
  return frames;
}

export function architectSummonSyncDeclaration(targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: true,
    animationTiles: [
      {x: targetX, y: targetY, char: 'X', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}
