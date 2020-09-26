import { Actor } from '../actor.object';
import { PlayerSynchronization } from 'src/app/shared/models/synchronization/objects/player-synchronization.model';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';

export class Player {
  id: string;
  battlePlayerStatus: BattlePlayerStatusEnum;
  keyActors: number[];

  constructor(synchronizer: PlayerSynchronization) {
    this.id = synchronizer.id;
    this.battlePlayerStatus = synchronizer.battlePlayerStatus;
  }

  removeKeyActor(id: number) {
    removeFromArray(this.keyActors, id);
  }
}
