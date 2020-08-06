import { SynchronizationMessageType } from '../enum/synchronization-message-type.enum';
import { ActionInfo } from './action-info.model';
import { Synchronizer } from './synchronizer.model';
import { RewardInfo } from './reward-info.model';
import { FullSynchronizationInfo } from './full-synchronization-info.model';
import { StartTurnInfo } from './start-turn-info.model';

export interface SynchronizationMessageDto {
  id: SynchronizationMessageType;
  varsion: number;
  code: string;
  action: ActionInfo;
  reward: RewardInfo;
  startTurnInfo: StartTurnInfo;
}
