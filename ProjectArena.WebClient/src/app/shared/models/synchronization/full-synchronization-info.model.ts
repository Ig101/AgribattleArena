import { RewardInfo } from './reward-info.model';
import { ActorSynchronization } from './objects/actor-synchronization.model';

export interface FullSynchronizationInfo {
  reward: RewardInfo;
  id: string;
  actors: ActorSynchronization[];
  width: number;
  height: number;
  // TODO FullInfo
}
