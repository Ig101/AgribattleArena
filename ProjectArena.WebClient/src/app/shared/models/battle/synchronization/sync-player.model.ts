import { BattlePlayerStatusEnum } from '../../enum/player-battle-status.enum';

export interface SyncPlayer {
  id: string;
  userId: string;
  team?: number;
  keyActorsSync: number[];
  turnsSkipped: number;
  status: BattlePlayerStatusEnum;
}
