import { BattlePlayerStatusEnum } from '../../enum/player-battle-status.enum';

export interface SyncPlayer {
  id: string;
  team?: number;
  keyActorsSync: number[];
  turnsSkipped: number;
  status: BattlePlayerStatusEnum;
}
