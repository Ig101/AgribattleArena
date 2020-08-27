import { Reaction } from '../models/abstract/reaction.model';
import { Action } from '../models/abstract/action.model';
import { ActionSynchronization } from 'src/app/shared/models/synchronization/objects/action-synchronization.model';
import { BuffSynchronization } from 'src/app/shared/models/synchronization/objects/buff-synchronization.model';
import { Buff } from '../models/abstract/buff.model';
import { ReactionSynchronization } from 'src/app/shared/models/synchronization/objects/reaction-synchronization.model';

export interface INativesCollection {
  buildReaction(synchronization: ReactionSynchronization): Reaction;
  buildAction(synchronization: ActionSynchronization): Action;
  buildBuff(synchronization: BuffSynchronization): Buff;
}
