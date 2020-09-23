import { RewardInfo } from './reward-info.model';
import { ActorSynchronization } from './objects/actor-synchronization.model';
import { PlayerSynchronization } from './objects/player-synchronization.model';
import { FinishSceneMessage } from './finish-scene-message.model';
import { BiomEnum } from '../enum/biom.enum';

export interface FullSynchronizationInfo {
  id: string;
  timeLine: number;
  idCounterPosition: number;
  currentPlayerId: string;
  actors: ActorSynchronization[];
  players: PlayerSynchronization[];
  width: number;
  height: number;
  biom: BiomEnum;
  waitingActions: FinishSceneMessage[];
}
