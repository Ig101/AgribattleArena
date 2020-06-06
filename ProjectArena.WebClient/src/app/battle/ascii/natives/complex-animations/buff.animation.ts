import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';

export function buffIssueDeclaration(targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX - 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY + 1, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY - 1, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX - 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY + 1, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: false, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX - 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY + 1, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX - 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: false, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX - 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true},
      {x: targetX, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: false, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX + 1, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: true, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [
      {x: targetX, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: false, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  frames.push({
    updateSynchronizer: false,
    animationTiles: [],
    specificAction: undefined
  });

  return frames;
}

export function buffSyncDeclaration(targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: true,
    animationTiles: [
      {x: targetX, y: targetY, char: '+', color, unitAlpha: false,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: false, overflowHealth: false, workingOnSpecEffects: true}],
    specificAction: undefined
  });
  return frames;
}
