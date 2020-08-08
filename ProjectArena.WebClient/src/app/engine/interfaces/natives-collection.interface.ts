import { Reaction } from '../models/abstract/reaction.model';
import { Action } from '../models/abstract/action.model';
import { ActionSynchronization } from 'src/app/shared/models/synchronization/objects/action-synchronization.model';
import { BuffSynchronization } from 'src/app/shared/models/synchronization/objects/buff-synchronization.model';
import { Buff } from '../models/abstract/buff.model';

export interface INativesCollection {
  buildReaction(id: string): Reaction;
  buildAction(synchronization: ActionSynchronization): Action;
  buildBuff(synchronization: BuffSynchronization): Buff;
}
