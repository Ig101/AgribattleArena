import { ActionNative } from '../models/action-native.model';
import { ActionClassEnum } from 'src/app/engine/models/enums/action-class.enum';
import { Actor } from 'src/app/engine/scene/actor.object';
import { moveValidation, moveAction } from './move.native';
import { attackAction } from './attack.native';
import { shotAction } from './ranged-attack.native';

export const actionNatives: { [id: string]: ActionNative } = {
  move: {
    char: undefined,
    name: undefined,
    untargeted: false,
    onlyVisible: false,
    description: undefined,
    timeCost: 3,
    cooldown: 0,
    power: 0,
    aiPriority: 1,
    actionClass: ActionClassEnum.Move,
    validateActionTargeted: moveValidation,
    actionTargeted: moveAction
  },
  slash: {
    char: undefined,
    name: undefined,
    untargeted: false,
    onlyVisible: false,
    description: undefined,
    timeCost: 6,
    cooldown: 0,
    power: 12,
    aiPriority: 1,
    actionClass: ActionClassEnum.Attack,
    actionOnObject: attackAction
  },
  wait: {
    char: 'W',
    name: 'Wait',
    untargeted: true,
    onlyVisible: false,
    description: 'Do nothing for one turn',
    timeCost: 30,
    cooldown: 0,
    power: 0,
    aiPriority: 1,
    actionClass: ActionClassEnum.Default,
    actionTargeted: (actor: Actor, power: number, x: number, y: number, startingTime: number) => {
      actor.parentActor.handleEffects(['wait-act'], power, true, 1, startingTime);
      actor.handleEffects(['wait-act'], power, false, 1, startingTime);
      return [];
    }
  },
  shot: {
    char: 'S',
    name: 'Shot',
    untargeted: false,
    onlyVisible: true,
    description: 'Deal physical damage to a visible target',
    timeCost: 12,
    cooldown: 0,
    power: 20,
    aiPriority: 1,
    actionClass: ActionClassEnum.Default,
    actionOnObject: shotAction,
  }
};
