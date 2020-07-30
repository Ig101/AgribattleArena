import { TileNative } from '../models/natives/tile-native.model';

export const tileNatives: { [id: string]: TileNative } = {
  grass: {
    name: 'Grass',
    description: 'No special effects',
    visualization: {
      char: 'grass',
      color: { r: 45, g: 60, b: 150, a: 1 }
    },
    backgroundColor: { r: 16, g: 22, b: 50 },
    bright: false,
    action: undefined,
    onActionEffectAnimation: undefined,
    onStepAction: undefined
  },
  rock: {
    name: 'Rock',
    description: 'Unpassable barrier',
    visualization: {
      char: 'rock',
      color: { r: 72, g: 77, b: 123, a: 1 },
    },
    backgroundColor: { r: 16, g: 22, b: 50 },
    bright: false,
    action: undefined,
    onActionEffectAnimation: undefined,
    onStepAction: undefined
  },
  powerplace: {
    name: 'Place of power',
    description: 'Increases strength and willpower of standing character.',
    visualization: {
      char: 'o',
      color: { r: 155, g: 155, b: 255, a: 1 }
    },
    backgroundColor: { r: 35, g: 35, b: 100 },
    bright: false,
    action: undefined,
    onActionEffectAnimation: undefined,
    onStepAction: undefined
  },
};
