import { Injectable } from '@angular/core';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
import { StartTurnInfo } from 'src/app/shared/models/synchronization/start-turn-info.model';
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';

@Injectable()
export class SceneService {

  constructor() { }

  setupGame(fullSynchronizer: FullSynchronizationInfo) {

  }

  act(actionInfo: ActionInfo, isReceivedFromMessage: boolean) {

  }

  startTurn(definition: StartTurnInfo) {

  }

  setupRewardInfo(reward: RewardInfo) {

  }

  endGame(reward: RewardInfo, victorious: boolean) {

  }
}
