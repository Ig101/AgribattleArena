import { Actor } from '../actor.object';
import { PlayerSynchronization } from 'src/app/shared/models/synchronization/objects/player-synchronization.model';

export class Player {
  id: string;
  isBot: boolean;

  constructor(synchronizer: PlayerSynchronization) {
    this.id = synchronizer.id;
    this.isBot = synchronizer.isBot;
  }
}
