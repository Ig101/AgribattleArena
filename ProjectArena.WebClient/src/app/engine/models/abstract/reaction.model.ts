import { Actor } from '../../scene/actor.object';
import { Observer } from 'rxjs';
import { ChangeDefinition } from './change-definition.model';
import { ReactionNative } from 'src/app/content/models/reaction-native.model';

export const KILL_EFFECT_NAME = 'kill';

export interface Reaction {
  id: string;
  mod: number;
  native: ReactionNative;
}
