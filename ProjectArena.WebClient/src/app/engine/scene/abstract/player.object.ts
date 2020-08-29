import { Actor } from '../actor.object';
import { PlayerSynchronization } from 'src/app/shared/models/synchronization/objects/player-synchronization.model';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';

export class Player {
  id: string;
  battlePlayerStatus: BattlePlayerStatusEnum;

  constructor(synchronizer: PlayerSynchronization) {
    this.id = synchronizer.id;
    this.battlePlayerStatus = synchronizer.battlePlayerStatus;
  }
}
