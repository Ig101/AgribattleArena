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
    onStepAction: undefined
  }
};
