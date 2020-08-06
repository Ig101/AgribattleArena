import { Injectable } from '@angular/core';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
import { StartTurnInfo } from 'src/app/shared/models/synchronization/start-turn-info.model';
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';
import { Subject } from 'rxjs';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';
import { SynchronizationMessageDto } from 'src/app/shared/models/synchronization/synchronization-message-dto.model';
import { Scene } from '../scene/scene.object';

@Injectable()
export class SceneService {

  actionsSub = new Subject<ActionInfo>();
  synchronizersSub = new Subject<Synchronizer>();

  private scene: Scene;

  constructor() { }

  setupGame(fullSynchronizer: FullSynchronizationInfo) {

  }

  act(actionInfo: ActionInfo) {

  }

  processMessage(message: SynchronizationMessageDto) {
    if (this.scene) {
      this.scene.pushMessages(message);
    }
  }
}
