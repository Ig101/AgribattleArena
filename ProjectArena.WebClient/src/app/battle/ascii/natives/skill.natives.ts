import { SkillNative } from '../models/natives/skill-native.model';
import { AnimationFrame } from '../models/animations/animation-frame.model';
import { Color } from 'src/app/shared/models/color.model';
import { animationFrame } from 'rxjs/internal/scheduler/animationFrame';
import { AnimationTile } from '../models/animations/animation-tile.model';
import { explosionIssueDeclaration } from './complex-animations/explosion.animation';
import { throwIssueDeclaration, arrowThrowIssueDeclaration } from './complex-animations/throw.animation';
import { chargeIssueDeclaration, chargeSyncDeclaration } from './complex-animations/charge.animation';
import { instantSyncDeclaration } from './complex-animations/instant.animation';
import { architectSummonIssueDeclaration, architectSummonSyncDeclaration } from './complex-animations/architect-summon.animation';
import { strikeSyncDeclaration, strikeIssueDeclaration } from './complex-animations/strike.animation';
import { undirectDamageIssueDeclaration, undirectDamageSyncDeclaration } from './complex-animations/undirect-damage.animation';
import { summonIssueAnimation, summonSyncDeclaration } from './complex-animations/summon.animation';
import { buffIssueDeclaration, buffSyncDeclaration } from './complex-animations/buff.animation';

export const skillNatives: { [id: string]: SkillNative } = {
  slash: {
    name: 'Slash',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return strikeIssueDeclaration(tile.x, tile.y);
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
    alternativeForm: false
  },
  mistSlash: {
    name: 'Mist Slash',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return strikeIssueDeclaration(tile.x, tile.y);
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
    alternativeForm: false
  },
  magicMissle: {
    name: 'Magic missle',
    description: 'Throws a missle of pure mist magic to the target that deals medium damage.',
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return throwIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, '*', {r: 80, g: 220, b: 255, a: 1});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
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
        return throwIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, 'o', {r: 200, g: 200, b: 200, a: 1});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
    alternativeForm: false
  },
  shot: {
    name: 'Shot',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return arrowThrowIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, {r: 200, g: 200, b: 200, a: 1});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
    alternativeForm: false
  },
  mistShot: {
    name: 'Mist Shot',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return arrowThrowIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, {r: 140, g: 190, b: 200, a: 1});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
    alternativeForm: false
  },
  mistwalk: {
    name: 'Mistwalk',
    description: 'Teleports to the target location.',
    action: {
      generateIssueDeclarations: undefined,
      generateSyncDeclarations: (issuer, tile) => {
        return instantSyncDeclaration(issuer.x, issuer.y, tile.x, tile.y, {r: 100, g: 100, b: 255, a: 1});
      },
    },
    alternativeForm: false
  },
  powerplace: {
    name: 'Place of power',
    description: `Creates a place of power on the target position that increases strangth and willpower by 100%
      of its default value. Works only on characters that stay on chosen position.`,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return architectSummonIssueDeclaration(tile.x, tile.y, {r: 20, g: 20, b: 135, a: 1}, {r: 16, g: 16, b: 8});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return architectSummonSyncDeclaration(tile.x, tile.y, {r: 140, g: 140, b: 255, a: 1});
      }
    },
    alternativeForm: false
  },
  barrier: {
    name: 'Barrier',
    description: `Creates an unpassable barrier on the target position with large amount of health.`,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return architectSummonIssueDeclaration(tile.x, tile.y, {r: 54, g: 32, b: 0, a: 1}, {r: 16, g: 8, b: 0});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return architectSummonSyncDeclaration(tile.x, tile.y, {r: 174, g: 92, b: 0, a: 1});
      }
    },
    alternativeForm: false
  },
  wand: {
    name: 'Wand',
    description: undefined,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return undirectDamageIssueDeclaration(issuer.x, issuer.y, '*', {r: 80, g: 220, b: 255, a: 1});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return undirectDamageSyncDeclaration(tile.x, tile.y, '*', {r: 80, g: 220, b: 255, a: 1});
      },
    },
    alternativeForm: false
  },
  mistpact: {
    name: 'Mist pact',
    description: 'Summons lesser mist spawn that fights for you.',
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return summonIssueAnimation(issuer.x, issuer.y, tile.x, tile.y, { r: 15, g: 10, b: 135, a: 1 }, { r: 16, g: 12, b: 16});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return summonSyncDeclaration(tile.x, tile.y, { r: 135, g: 100, b: 255, a: 1 });
      },
    },
    alternativeForm: true,
    enemyName: 'Mist pact',
    enemyAction: {
      generateIssueDeclarations: (issuer, tile) => {
        return summonIssueAnimation(issuer.x, issuer.y, tile.x, tile.y, { r: 10, g: 10, b: 135, a: 1 }, { r: 6, g: 6, b: 16});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return summonSyncDeclaration(tile.x, tile.y, { r: 55, g: 55, b: 255, a: 1 });
      },
    }
  },
  bloodsphere: {
    name: 'Blood sphere',
    description: 'Throws an infused with blood magic missle to the target that deals medium damage. Costs 5 health.',
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return throwIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, '*', {r: 255, g: 0, b: 0});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
    alternativeForm: true,
    enemyName: 'Blood sphere',
    enemyAction: {
      generateIssueDeclarations: (issuer, tile) => {
        return throwIssueDeclaration(issuer.x, issuer.y, tile.x, tile.y, '*', {r: 55, g: 55, b: 255});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return strikeSyncDeclaration(tile.x, tile.y);
      },
    },
  },
  offspring: {
    name: 'Offspring',
    description: `Creates entity from caster\'s blood using 20% of health.
      Can attack and sacrifice itself transferring all health to the target. Doesn\'t have intelligence by self.`,
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return summonIssueAnimation(issuer.x, issuer.y, tile.x, tile.y, { r: 135, g: 0, b: 0, a: 1 }, { r: 16, g: 0, b: 0});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return summonSyncDeclaration(tile.x, tile.y, { r: 255, g: 0, b: 0, a: 1 });
      },
    },
    alternativeForm: true,
    enemyName: 'Offspring',
    enemyAction: {
      generateIssueDeclarations: (issuer, tile) => {
        return summonIssueAnimation(issuer.x, issuer.y, tile.x, tile.y, { r: 10, g: 10, b: 135, a: 1 }, { r: 6, g: 6, b: 16});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return summonSyncDeclaration(tile.x, tile.y, { r: 55, g: 55, b: 255, a: 1 });
      },
    }
  },
  sacrifice: {
    name: 'Sacrifice',
    description: 'Transfers all health to the target. Doesn\'t work on structures and mechanisms.',
    action: {
      generateIssueDeclarations: undefined,
      generateSyncDeclarations: (issuer, tile) => {
        return instantSyncDeclaration(issuer.x, issuer.y, tile.x, tile.y, {r: 255, g: 0, b: 0, a: 1});
      },
    },
    alternativeForm: true,
    enemyName: 'Sacrifice',
    enemyAction: {
      generateIssueDeclarations: undefined,
      generateSyncDeclarations: (issuer, tile) => {
        return instantSyncDeclaration(issuer.x, issuer.y, tile.x, tile.y, {r: 55, g: 55, b: 255, a: 1});
      },
    }
  },
  empower: {
    name: 'Empower',
    description: 'Places an empower buff on the target for 3 turn. Increases actions effectiveness by 50% of willpower modifier.',
    action: {
      generateIssueDeclarations: (issuer, tile) => {
        return  buffIssueDeclaration(tile.x, tile.y, {r: 80, g: 220, b: 255, a: 1});
      },
      generateSyncDeclarations: (issuer, tile) => {
        return buffSyncDeclaration(tile.x, tile.y, {r: 80, g: 220, b: 255, a: 1});
      },
    },
    alternativeForm: false
  }
};
