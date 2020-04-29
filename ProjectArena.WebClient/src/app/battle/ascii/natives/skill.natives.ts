import { SkillNative } from '../models/natives/skill-native.model';

export const skillNatives: { [id: string]: SkillNative } = {
  slash: {
    name: 'Slash',
    description: 'Strike with character\'s weapon',
    action: {

    },
    alternativeForm: false
  },
  explosion: {
    name: 'Explosion',
    description: 'Strike area 3x3 with fire and stun affected characters for 1 turn',
    action: {

    },
    alternativeForm: true,
    enemyName: 'Explosion',
    enemyAction: {

    }
  }
};
