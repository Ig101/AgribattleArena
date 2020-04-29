import { ActorNative } from '../models/natives/actor-native.model';

export const actorNatives: {[id: string]: ActorNative} = {
  adventurer: {
    name: 'Adventurer',
    description: 'Human warrior',
    visualization: {
      char: '@',
      color: { r: 255, g: 55, b: 0, a: 1 }
    },
    moveAction: {

    },
    enemyName: 'Demonspawn',
    enemyDescription: 'Blue plague warrior',
    enemyVisualization: {
      char: 'D',
      color: { r: 55, g: 55, b: 255, a: 1 }
    },
    enemyMoveAction: {

    }
  }
};
