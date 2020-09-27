import { BattlePlayerStatusEnum } from '../../enum/player-battle-status.enum';
import { ActorReference } from './actor-reference.model';

export interface PlayerSynchronization {
  id: string;
  battlePlayerStatus: BattlePlayerStatusEnum;
  keyActors: ActorReference[];
}
