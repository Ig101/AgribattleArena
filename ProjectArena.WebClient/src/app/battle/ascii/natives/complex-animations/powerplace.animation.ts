import { AnimationFrame } from '../../models/animations/animation-frame.model';

export function powerplaceIssueAnimation(targetX: number, targetY: number) {
  const frames: AnimationFrame[] = [];
  const firstColor = { r: 5, g: 5, b: 105 };
  for (let i = 0; i < 15; i++) {
    const frameColor = { r: firstColor.r + 10 * i, g: firstColor.g + 10 * i, b: firstColor.b + 10 * i, a: 1};
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
  frames.push({
    updateSynchronizer: true,
    animationTiles: [
      {x: targetX, y: targetY, char: 'X', color: {r: 155, g: 155, b: 255, a: 1}, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}
