import { Injectable, Inject } from '@angular/core';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
import { StartTurnInfo } from 'src/app/shared/models/synchronization/start-turn-info.model';
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';
import { Subject, BehaviorSubject } from 'rxjs';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';
import { SynchronizationMessageDto } from 'src/app/shared/models/synchronization/synchronization-message-dto.model';
import { Scene, SCENE_FRAME_TIME } from '../scene/scene.object';
import { MessageType } from '@aspnet/signalr';
import { SynchronizationMessageType } from 'src/app/shared/models/enum/synchronization-message-type.enum';
import { NativesCollection } from 'src/app/content/natives-collection';

@Injectable()
export class SceneService {

  actionsSub = new Subject<ActionInfo>();
  synchronizersSub = new Subject<Synchronizer>();
  desyncSub = new BehaviorSubject<boolean>(false);
  endGameSub = new BehaviorSubject<boolean>(false);

  updateSub = new Subject<void>();
  updater;

  private sceneInternal: Scene;

  get scene() {
    return this.sceneInternal;
  }

  // TODO Inject token for natives collection
  constructor(
    private nativesCollection: NativesCollection
  ) { }

  setupGame(fullSynchronizer: FullSynchronizationInfo, reward: RewardInfo, turnInfo: StartTurnInfo) {
    this.sceneInternal = new Scene(
      this.actionsSub,
      this.synchronizersSub,
      this.desyncSub,
      this.nativesCollection,
      this.endGameSub,
      fullSynchronizer,
      reward,
      turnInfo
    );
  }

  startUpdates() {
    clearInterval(this.updater);
    this.scene.lastTime = performance.now();
    this.updater = setInterval(() => {
      this.scene.update();
      this.updateSub.next();
    }, SCENE_FRAME_TIME);
  }

  clearScene() {
    clearInterval(this.updater);
    this.sceneInternal = undefined;
  }

  processMessage(message: SynchronizationMessageDto) {
    if (this.scene) {
      this.scene.pushMessages(message);
    }
  }
}
