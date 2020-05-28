import { BuffNative } from '../models/natives/buff-native.model';

export const buffNatives: { [id: string]: BuffNative } = {
  stun: {
    char: 's',
    color: { r: 100, g: 100, b: 250 },
    name: 'Stun',
    description: 'Character cannot do anything and doesn\'t receive action points.',
    onApplyAnimation: undefined,
    effectAnimation: undefined,
    onActionEffectAnimation: undefined,
    onPurgeAnimation: undefined,
    passiveAnimation: {
      doSomethingWithBearer: (action, bearer) => {
        const colorChangeR = Math.min(bearer.defaultVisualization.color.r * 0.4,
          bearer.visualization.color.r - bearer.defaultVisualization.color.r * 0.1);
        const colorChangeG = Math.min(bearer.defaultVisualization.color.g * 0.4,
          bearer.visualization.color.g - bearer.defaultVisualization.color.g * 0.1);
        const colorChangeB = Math.min(bearer.defaultVisualization.color.b * 0.4,
          bearer.visualization.color.b - bearer.defaultVisualization.color.b * 0.1);
        bearer.visualization.color.r -= colorChangeR;
        bearer.visualization.color.g -= colorChangeG;
        bearer.visualization.color.b -= colorChangeB;
        action.colorDifference.r = colorChangeR;
        action.colorDifference.g = colorChangeG;
        action.colorDifference.b = colorChangeB;
      },
      resetEffect: (action, bearer) => {
        bearer.visualization.color.r += action.colorDifference.r;
        bearer.visualization.color.g += action.colorDifference.g;
        bearer.visualization.color.b += action.colorDifference.b;
      }
    }
  },
  tilepower: {
    char: 'p',
    color: { r: 255, g: 255, b: 100 },
    name: 'Place of power',
    description: 'Character have increased strength and willpower.',
    onApplyAnimation: undefined,
    effectAnimation: undefined,
    onActionEffectAnimation: undefined,
    onPurgeAnimation: undefined,
    passiveAnimation: undefined
  },
  empower: {
    char: 'e',
    color: { r: 255, g: 100, b: 255 },
    name: 'Empower',
    description: 'Character\'s actions are more effective.',
    onApplyAnimation: undefined,
    effectAnimation: undefined,
    onActionEffectAnimation: undefined,
    onPurgeAnimation: undefined,
    passiveAnimation: undefined
  },
};
