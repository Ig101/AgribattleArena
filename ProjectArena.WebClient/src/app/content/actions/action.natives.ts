import { ActionNative } from '../models/action-native.model';
import { ActionClassEnum } from 'src/app/engine/models/enums/action-class.enum';
import { Actor } from 'src/app/engine/scene/actor.object';
import { moveValidation, moveAction } from './move.native';
import { attackAction } from './attack.native';
import { shotAction } from './ranged-attack.native';

export const actionNatives: { [id: string]: ActionNative } = {
  move: {
    name: undefined,
    untargeted: false,
    onlyVisible: false,
    description: undefined,
    timeCost: 2,
    cooldown: 0,
    power: 1,
    aiPriority: 1,
    actionClass: ActionClassEnum.Move,
    validateActionTargeted: moveValidation,
    actionTargeted: moveAction
  },
  slash: {
    name: undefined,
    untargeted: false,
    onlyVisible: false,
    description: undefined,
    timeCost: 8,
    cooldown: 0,
    power: 12,
    aiPriority: 1,
    actionClass: ActionClassEnum.Attack,
    actionOnObject: attackAction
  },
  wait: {
    name: 'Wait',
    untargeted: true,
    onlyVisible: false,
    description: 'Do nothing for one turn',
    timeCost: 6,
    cooldown: 0,
    power: 1,
    aiPriority: 1,
    actionClass: ActionClassEnum.Default,
    actionTargeted: (actor: Actor, power: number, x: number, y: number, startingTime: number) => []
  },
  shot: {
    name: 'Shot',
    untargeted: false,
    onlyVisible: true,
    description: 'Shot missle to target location, dealing damage to the highest large object',
    timeCost: 18,
    cooldown: 0,
    power: 20,
    aiPriority: 1,
    actionClass: ActionClassEnum.Default,
    actionTargeted: shotAction,
  }
};
