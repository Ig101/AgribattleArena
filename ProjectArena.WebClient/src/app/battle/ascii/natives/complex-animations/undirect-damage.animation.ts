import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { Color } from 'src/app/shared/models/color.model';

export function undirectDamageIssueDeclaration(issueX: number, issueY: number, char: string, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: issueX - 1, y: issueY - 1, char: '/', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: issueX + 1, y: issueY + 1, char: '/', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: issueX - 1, y: issueY + 1, char: '\\', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: issueX + 1, y: issueY - 1, char: '\\', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [{x: issueX, y: issueY, char, color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}

export function undirectDamageSyncDeclaration(targetX: number, targetY: number, char: string, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: false,
    animationTiles: [{x: targetX, y: targetY, char, color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: true,
    animationTiles: [{x: targetX, y: targetY, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}],
    specificAction: undefined
  });
  return frames;
}
