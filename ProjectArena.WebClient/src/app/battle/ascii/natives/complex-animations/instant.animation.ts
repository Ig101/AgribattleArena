import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { rangeBetween, angleBetween } from 'src/app/helpers/math.helper';

export function instantIssueDeclaration() {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: false,
    animationTiles: [],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [],
    specificAction: undefined
  });
  return frames;
}

export function instantSyncDeclaration(issueX: number, issueY: number, targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: true,
    animationTiles: [
      {x: targetX, y: targetY, char: '*', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: issueX, y: issueY, char: '*', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}
