import { RewardInfo } from './reward-info.model';
import { ActorSynchronization } from './objects/actor-synchronization.model';
import { PlayerSynchronization } from './objects/player-synchronization.model';
import { FinishSceneMessage } from './finish-scene-message.model';
import { BiomEnum } from '../enum/biom.enum';
import { ActorReference } from './objects/actor-reference.model';

export interface FullSynchronizationInfo {
  id: string;
  timeLine: number;
  automatic: boolean;
  idCounterPosition: number;
  currentPlayer: string;
  actors: ActorSynchronization[];
  players: PlayerSynchronization[];
  width: number;
  height: number;
  biom: BiomEnum;
}
