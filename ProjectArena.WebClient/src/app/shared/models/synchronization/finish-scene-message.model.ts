import { SynchronizationMessageType as FinishSceneMessageType } from '../enum/finish-scene-message-type.enum';
import { FullSynchronizationInfo } from './full-synchronization-info.model';
import { RewardInfo } from './reward-info.model';

export interface FinishSceneMessage {
  id: FinishSceneMessageType;
  sceneId: string;
  code: string;
  reward: RewardInfo;
  fullSynchronization: FullSynchronizationInfo;
}
