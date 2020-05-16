import { ActorNative } from '../models/natives/actor-native.model';

export const actorNatives: {[id: string]: ActorNative} = {
  adventurer: {
    name: 'Adventurer',
    description: 'Adventurer',
    visualization: {
      char: '@',
      color: { r: 160, g: 160, b: 160, a: 1 }
    },
    enemyName: 'Mistspawn',
    enemyDescription: 'Blue plague soldier',
    enemyVisualization: {
      char: 'S',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  }
};
