import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { Actor } from './scene/actor.model';

export interface Player {
  id: string;
  team?: number;
  name: string;
  keyActors: number[];
  turnsSkipped: number;
  status: BattlePlayerStatusEnum;
}
