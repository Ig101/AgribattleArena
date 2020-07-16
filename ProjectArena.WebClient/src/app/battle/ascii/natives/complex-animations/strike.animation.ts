import { AnimationFrame } from '../../models/animations/animation-frame.model';

export function strikeIssueDeclaration(targetX: number, targetY: number) {
  const frames: AnimationFrame[] = [];
  for (let i = 0; i < 3; i++) {
    frames.push({
      updateSynchronizer: false,
      animationTiles: [],
      specificAction: undefined
    });
  }
  for (let i = 0; i < 2; i++) {
    frames.push({
      updateSynchronizer: false,
      animationTiles: [{x: targetX, y: targetY, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
        unitColorMultiplier: i % 2, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}],
      specificAction: undefined
    });
  }
  return frames;
}

export function strikeSyncDeclaration(targetX: number, targetY: number) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: true,
    animationTiles: [{x: targetX, y: targetY, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}],
    specificAction: undefined
  });
  return frames;
}
