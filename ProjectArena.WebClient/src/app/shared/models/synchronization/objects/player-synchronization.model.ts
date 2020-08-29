import { BattlePlayerStatusEnum } from '../../enum/player-battle-status.enum';

export interface PlayerSynchronization {
  id: string;
  battlePlayerStatus: BattlePlayerStatusEnum;
}
