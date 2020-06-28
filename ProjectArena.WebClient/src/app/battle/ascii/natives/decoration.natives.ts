import { DecorationNative } from '../models/natives/decoration-native.model';


export const decorationNatives: { [id: string]: DecorationNative } = {
  barrier: {
    name: 'Barrier',
    description: 'Unpassable barrier.',
    active: false,
    visualization: {
      char: '#',
      color: {r: 174, g: 92, b: 0, a: 1}
    },
    action: undefined,
    onDeathAction: undefined
  },
  tree: {
    name: 'Tree',
    description: 'Unpassable barrier.',
    active: false,
    visualization: {
      char: 'Y',
      color: { r: 139, g: 69, b: 19, a: 1 }
    },
    action: undefined,
    onDeathAction: undefined
  }
};
