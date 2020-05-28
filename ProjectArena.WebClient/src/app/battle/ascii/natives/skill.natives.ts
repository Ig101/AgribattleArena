import { SkillNative } from '../models/natives/skill-native.model';
import { AnimationFrame } from '../models/animations/animation-frame.model';
import { Color } from 'src/app/shared/models/color.model';
import { animationFrame } from 'rxjs/internal/scheduler/animationFrame';
import { AnimationTile } from '../models/animations/animation-tile.model';
import { explosionIssueDeclaration } from './complex-animations/explosion.animation';
import { throwIssueDeclaration, arrowThrowIssueDeclaration } from './complex-animations/throw.animation';
import { chargeIssueDeclaration, chargeSyncDeclaration } from './complex-animations/charge.animation';
import { shadowstepSyncDeclaration } from './complex-animations/shadowstep.animation';
import { powerplaceIssueAnimation } from './complex-animations/powerplace.animation';

export const skillNatives: { [id: string]: SkillNative } = {
  slash: {
    name: 'Slash',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        const frames: AnimationFrame[] = [];
        for (let i = 0; i < 3; i++) {
          frames.push({
            updateSynchronizer: false,
            animationTiles: [],
            specificAction: undefined
          });
        }
        for (let i = 0; i < 4; i++) {
          frames.push({
            updateSynchronizer: i === 3 ? true : false,
            animationTiles: [{x: tile.x, y: tile.y, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
              unitColorMultiplier: i % 2, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}],
            specificAction: undefined
          });
        }
        return frames;
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: false
  },
  magicMissle: {
    name: 'Magic missle',
    description: 'Throws a missle of pure mist magic to the target that deals medium damage.',
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return throwIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, '*', {r: 100, g: 100, b: 255});
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: false
  },
  charge: {
    name: 'Charge',
    description: `Charges into the target and deals slow damage to it.
      If the distance from target is more than 2, also stun it for 1 turn.`,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return chargeIssueDeclaration(issuer.x, issuer.y);
      },
      generateSyncDeclarations: (issuer, tile) => {
        return chargeSyncDeclaration(issuer.x, issuer.y, tile.x, tile.y, issuer.visualization.char, issuer.visualization.color);
      },
    },
    alternativeForm: false
  },
  warden: {
    name: 'Warden',
    description: 'Throws a shield to the target that deals medium damage.',
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return throwIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, 'o', {r: 200, g: 200, b: 200});
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: false
  },
  shot: {
    name: 'Shot',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return arrowThrowIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, {r: 200, g: 200, b: 200});
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: false
  },
  shadowstep: {
    name: 'Shadowstep',
    description: 'Teleports to the target location.',
    action: {
      generateIssueDeclarations: undefined,
      generateSyncDeclarations: (issuer, tile) => {
        return shadowstepSyncDeclaration(issuer.x, issuer.y, tile.x, tile.y);
      },
    },
    alternativeForm: false
  },
  powerplace: {
    name: 'Place of power',
    description: `Creates a place of power on the target position that increases strange and willpower by 100%
      of its default value. Works only on characters that stay on chosen position.`,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return powerplaceIssueAnimation(tile.x, tile.y);
      },
      generateSyncDeclarations: undefined
    },
    alternativeForm: false
  }
};
