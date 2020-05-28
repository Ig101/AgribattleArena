import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { rangeBetween, angleBetween } from 'src/app/helpers/math.helper';

export function shadowstepSyncDeclaration(issueX: number, issueY: number, targetX: number, targetY: number) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: true,
    animationTiles: [
      {x: targetX, y: targetY, char: '*', color: {r: 100, g: 100, b: 255, a: 1}, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: issueX, y: issueY, char: '*', color: {r: 100, g: 100, b: 255, a: 1}, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}
