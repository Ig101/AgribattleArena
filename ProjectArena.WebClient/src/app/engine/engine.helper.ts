import { Action } from './models/abstract/action.model';

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
