import { Actor } from '../actor.object';
import { PlayerSynchronization } from 'src/app/shared/models/synchronization/objects/player-synchronization.model';

export class Player {
  id: string;

  constructor(synchronizer: PlayerSynchronization) {
    this.id = synchronizer.id;
  }
}
