import { SkillNative } from '../models/natives/skill-native.model';
import { AnimationFrame } from '../models/animations/animation-frame.model';
import { Color } from 'src/app/shared/models/color.model';
import { animationFrame } from 'rxjs/internal/scheduler/animationFrame';
import { AnimationTile } from '../models/animations/animation-tile.model';

function explosionIssueDeclaration(targetX, targetY, color: Color) {
  const colors = new Array<Color>(4);
  for (let i = 0; i < 4; i++) {
    colors[i] = {r: (Math.random() + 9) / 10 * color.r, g: (Math.random() + 9) / 10 * color.g, b: (Math.random() + 9) / 10 * color.b, a: 1};
  }
  const frames: AnimationFrame[] = [];
  frames.push({
    updateSynchronizer: false,
    animationTiles: [{x: targetX, y: targetY, char: '*', color: colors[0], unitColorMultiplier: 0,
      unitAlpha: false, ignoreHeight: false, overflowHealth: true, priority: 10, workingOnSpecEffects: true}]
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
      animationTiles: tiles
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
      animationTiles: tiles
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
    animationTiles: newTiles
  });
  return frames;
}

export const skillNatives: { [id: string]: SkillNative } = {
  slash: {
    name: 'Slash',
    description: 'Strikes with character\'s weapon',
    action: {
      generateIssueDeclarations: (x, y, targetX, targetY) => {
        const frames: AnimationFrame[] = [];
        for (let i = 0; i < 3; i++) {
          frames.push({
            updateSynchronizer: false,
            animationTiles: []
          });
        }
        for (let i = 0; i < 5; i++) {
          frames.push({
            updateSynchronizer: i === 4 ? true : false,
            animationTiles: [{x: targetX, y: targetY, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
              unitColorMultiplier: 0.2 * i, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}]
          });
        }
        return frames;
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: false
  },
  explosion: {
    name: 'Explosion',
    description: 'Strikes area 3x3 with fire and stun affected characters for 1 turn',
    action: {
      generateIssueDeclarations: (x, y, targetX, targetY) => {
        return explosionIssueDeclaration(targetX, targetY, {r: 255, g: 55, b: 0});
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: true,
    enemyName: 'Explosion',
    enemyAction: {
      generateIssueDeclarations: (x, y, targetX, targetY) => {
        return explosionIssueDeclaration(targetX, targetY, {r: 0, g: 55, b: 255});
      },
      generateSyncDeclarations: undefined
    }
  }
};
