import { TileNative } from '../models/natives/tile-native.model';

export const tileNatives: { [id: string]: TileNative } = {
  grass: {
    name: 'Grass',
    description: 'No special effects',
    visualization: {
      char: '-',
      color: { r: 0, g: 180, b: 0, a: 1 }
    },
    backgroundColor: { r: 0, g: 20, b: 0 },
    bright: false,
    action: undefined,
    onActionEffectAnimation: undefined,
    onStepAction: undefined
  },
  powerplace: {
    name: 'Place of power',
    description: 'Increases strength and willpower of standing character.',
    visualization: {
      char: '_',
      color: { r: 155, g: 155, b: 255, a: 1 }
    },
    backgroundColor: { r: 10, g: 10, b: 20 },
    bright: false,
    action: undefined,
    onActionEffectAnimation: undefined,
    onStepAction: undefined
  },
};
