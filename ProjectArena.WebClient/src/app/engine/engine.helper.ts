import { Action } from './models/abstract/action.model';

export const SCENE_FRAME_TIME = 1 / 30;

export function shiftTimeByFrames(time: number, frames: number) {
  return time + frames * SCENE_FRAME_TIME;
}


export function getMostPrioritizedAction(actions: Action[]) {
  if (!actions || actions.length === 0) {
    return undefined;
  }
  let action = actions[0];
  for (let i = 1; i < actions.length; i++) {
    if (actions[i].native.aiPriority > action.native.aiPriority) {
      action = actions[i];
    }
  }
  return action;
}
