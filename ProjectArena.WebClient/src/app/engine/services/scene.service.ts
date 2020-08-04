import { Injectable } from '@angular/core';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
import { StartTurnInfo } from 'src/app/shared/models/synchronization/start-turn-info.model';
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';
import { Subject } from 'rxjs';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';

@Injectable()
export class SceneService {

  actionsSub = new Subject<ActionInfo>();
  synchronizersSub = new Subject<Synchronizer>();

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
