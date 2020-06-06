import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { rangeBetween, angleBetween } from 'src/app/helpers/math.helper';

export function throwIssueDeclaration(issueX: number, issueY: number, targetX: number, targetY: number, char: string, color: Color) {
  const frames: AnimationFrame[] = [];
  const range = rangeBetween(issueX, issueY, targetX, targetY);
  const angle = angleBetween(issueX, issueY, targetX, targetY);
  const sin = Math.sin(angle);
  const cos = Math.cos(angle);
  for (let i = 0; i < range; i++) {
    const passed = i;
    const frameColor = {
      r: (Math.random() + 9) / 10 * color.r,
      g: (Math.random() + 9) / 10 * color.g,
      b: (Math.random() + 9) / 10 * color.b,
      a: 1};
    let x;
    let y;
    if (passed > range) {
      x = targetX;
      y = targetY;
    } else {
      x = Math.round(issueX + (passed * cos));
      y = Math.round(issueY + (passed * sin));
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: [{x, y, char, color: frameColor, unitColorMultiplier: 0,
        unitAlpha: false, ignoreHeight: false, overflowHealth: false, priority: 10, workingOnSpecEffects: true}],
      specificAction: undefined
    });
  }
  return frames;
}

export function arrowThrowIssueDeclaration(issueX: number, issueY: number, targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  const range = rangeBetween(issueX, issueY, targetX, targetY);
  const angle = angleBetween(issueX, issueY, targetX, targetY);
  const sin = Math.sin(angle);
  const cos = Math.cos(angle);
  for (let i = 0; i < range; i++) {
    const passed = i;
    const frameColor = {
      r: (Math.random() + 9) / 10 * color.r,
      g: (Math.random() + 9) / 10 * color.g,
      b: (Math.random() + 9) / 10 * color.b,
      a: 1};
    let x;
    let y;
    let char;
    if (passed > range) {
      x = targetX;
      y = targetY;
      char = '*';
    } else {
      x = Math.round(issueX + (passed * cos));
      y = Math.round(issueY + (passed * sin));
      let frameAngle = angleBetween(x, y, targetX, targetY);
      if (frameAngle > Math.PI * 2) {
        frameAngle -= Math.PI * 2;
      }
      if (frameAngle < 0) {
        frameAngle += Math.PI * 2;
      }
      if (frameAngle > Math.PI / 8 * 15 || frameAngle <= Math.PI / 8 ||
        frameAngle > Math.PI / 8 * 7 && frameAngle <= Math.PI / 8 * 9) {
        char = '-';
      }
      if (frameAngle > Math.PI / 8 && frameAngle <= Math.PI / 8 * 3 ||
        frameAngle > Math.PI / 8 * 9 && frameAngle <= Math.PI / 8 * 11) {
        char = '\\';
      }
      if (frameAngle > Math.PI / 8 * 3 && frameAngle <= Math.PI / 8 * 5 ||
        frameAngle > Math.PI / 8 * 11 && frameAngle <= Math.PI / 8 * 13) {
        char = '|';
      }
      if (frameAngle > Math.PI / 8 * 5 && frameAngle <= Math.PI / 7 ||
        frameAngle > Math.PI / 8 * 13 && frameAngle <= Math.PI / 8 * 15) {
        char = '/';
      }
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: [{x, y, char, color: frameColor, unitColorMultiplier: 0,
        unitAlpha: false, ignoreHeight: false, overflowHealth: false, priority: 10, workingOnSpecEffects: true}],
      specificAction: undefined
    });
  }
  return frames;
}
