import { ActorNative } from '../models/natives/actor-native.model';

export const actorNatives: {[id: string]: ActorNative} = {
  adventurer: {
    name: 'Adventurer',
    description: undefined,
    visualization: {
      char: '@',
      color: { r: 160, g: 160, b: 160, a: 1 }
    }
  },
  spawn: {
    name: 'Spawn',
    description: 'Blue mist soldier',
    visualization: {
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
    }
  },
  carver: {
    name: 'Carver',
    description: 'Blue mist support',
    visualization: {
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
    }
  },
  ritualist: {
    name: 'Ritualist',
    description: 'Blue mist mage',
    visualization: {
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
    }
  },
  blacksmith: {
    name: 'Blacksmith',
    description: 'Blue mist support',
    visualization: {
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
    }
  },
  fencer: {
    name: 'Fencer',
    description: 'Blue mist warrior',
    visualization: {
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
  },
  mage: {
    name: 'Mage',
    description: 'Blue mist mage',
    visualization: {
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
    }
  },
  impaler: {
    name: 'Impaler',
    description: 'Blue mist warrior',
    visualization: {
      char: 'I',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  tamedspawn: {
    name: 'Tamed mistspawn',
    description: 'Lesser spawn of blue mist',
    visualization: {
      char: 's',
      color: { r: 135, g: 100, b: 255, a: 1 }
    }
  },
  lesserspawn: {
    name: 'Lesser mistspawn',
    description: 'Lesser spawn of blue miste',
    visualization: {
      char: 's',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  },
  offspring: {
    name: 'Blood offspring',
    description: 'Living entity infused with blood',
    visualization: {
      char: 'o',
      color: { r: 255, g: 0, b: 0, a: 1 }
    }
  },
  mistoffspring: {
    name: 'Mist offspring',
    description: 'Living entity infused with mist',
    visualization: {
      char: 'o',
      color: { r: 55, g: 55, b: 255, a: 1 }
    }
  }
};
