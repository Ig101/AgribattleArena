import { ReactionNative } from '../models/reaction-native.model';
import { receiveDirectDamage } from './damage-reaction.native';

export const reactionNatives: { [id: string]: ReactionNative } = {
  physicalResponse: {
    respondsOn: 'physical-damage',
    action: (actor, power, mod, containerized, order, startingTime, issuer) =>
      receiveDirectDamage(actor, power, mod, containerized, startingTime)
  }
};
