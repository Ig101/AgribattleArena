import { RewardInfo } from './reward-info.model';
import { ActorSynchronization } from './objects/actor-synchronization.model';
import { PlayerSynchronization } from './objects/player-synchronization.model';
import { SynchronizationMessageDto } from './synchronization-message-dto.model';

export interface FullSynchronizationInfo {
  id: string;
  currentPlayerId: string;
  actors: ActorSynchronization[];
  players: PlayerSynchronization[];
  width: number;
  height: number;
  waitingActions: SynchronizationMessageDto[];
}
