import { Actor } from '../actor.object';
import { PlayerSynchronization } from 'src/app/shared/models/synchronization/objects/player-synchronization.model';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { Scene } from '../scene.object';

export class Player {
  id: string;
  battlePlayerStatus: BattlePlayerStatusEnum;
  keyActors: Actor[];

  constructor(synchronizer: PlayerSynchronization) {
    this.id = synchronizer.id;
    this.battlePlayerStatus = synchronizer.battlePlayerStatus;
  }

  removeKeyActor(actor: Actor) {
    removeFromArray(this.keyActors, actor);
  }
}
