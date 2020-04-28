import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { Actor } from './scene/actor.model';

export interface Player {
  id: string;
  team?: number;
  keyActors: Actor[];
  turnsSkipped: number;
  status: BattlePlayerStatusEnum;
}
