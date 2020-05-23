import { ActorNative } from '../models/natives/actor-native.model';

export const actorNatives: {[id: string]: ActorNative} = {
  adventurer: {
    name: 'Adventurer',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 160, g: 160, b: 160, a: 1 }
    },
    enemyName: 'Spawn',
    enemyDescription: 'Blue plague soldier',
    enemyVisualization: {
      char: 'S',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  architect: {
    name: 'Architect',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 165, g: 66, b: 0, a: 1 }
    },
    enemyName: 'Carver',
    enemyDescription: 'Blue plague support',
    enemyVisualization: {
      char: 'C',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  bloodletter: {
    name: 'Bloodletter',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 255, g: 0, b: 0, a: 1 }
    },
    enemyName: 'Ritualist',
    enemyDescription: 'Blue plague mage',
    enemyVisualization: {
      char: 'R',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  enchanter: {
    name: 'Enchanter',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 255, g: 40, b: 255, a: 1 }
    },
    enemyName: 'Blacksmith',
    enemyDescription: 'Blue plague support',
    enemyVisualization: {
      char: 'B',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  fighter: {
    name: 'Fighter',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 255, g: 132, b: 0, a: 1 }
    },
    enemyName: 'Fencer',
    enemyDescription: 'Blue plague warrior',
    enemyVisualization: {
      char: 'F',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  mistcaller: {
    name: 'Mistcaller',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 135, g: 100, b: 255, a: 1 }
    },
    enemyName: 'Mage',
    enemyDescription: 'Blue plague mage',
    enemyVisualization: {
      char: 'M',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  ranger: {
    name: 'Ranger',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 0, g: 255, b: 0, a: 1 }
    },
    enemyName: 'Impaler',
    enemyDescription: 'Blue plague warrior',
    enemyVisualization: {
      char: 'I',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
};
