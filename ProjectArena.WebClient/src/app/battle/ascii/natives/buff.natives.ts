import { BuffNative } from '../models/natives/buff-native.model';

export const buffNatives: { [id: string]: BuffNative } = {
  stun: {
    char: 's',
    color: { r: 100, g: 100, b: 250 },
    name: 'Stun',
    description: 'Character cannot do anything',
    onApplyAnimation: {

    },
    effectAnimation: undefined,
    onPurgeAnimation: undefined,
    passiveAnimation: {

    }
  }
};
