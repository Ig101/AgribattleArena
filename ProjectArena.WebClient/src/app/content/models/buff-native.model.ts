import { Action } from 'src/app/engine/models/abstract/action.model';
import { Reaction } from 'src/app/engine/models/abstract/reaction.model';

export interface BuffNative {
  name: string;
  description: string;
  updatesOnTurnOnly: boolean;

  blockedActions: string[];
  blockedEffects: string[];
  blockAllActions: boolean;
}
