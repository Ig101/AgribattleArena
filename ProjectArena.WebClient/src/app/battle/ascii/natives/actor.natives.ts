import { ActorNative } from '../models/natives/actor-native.model';

export const actorNatives: {[id: string]: ActorNative} = {
  adventurer: {
    name: 'Adventurer',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 255, g: 55, b: 0, a: 1 }
    },
    enemyName: 'Mistspawn',
    enemyDescription: 'Blue plague soldier',
    enemyVisualization: {
      char: 'S',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  }
};
