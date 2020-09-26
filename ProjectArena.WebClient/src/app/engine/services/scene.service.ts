import { Injectable, Inject } from '@angular/core';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';
import { Subject, BehaviorSubject } from 'rxjs';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';
import { FinishSceneMessage } from 'src/app/shared/models/synchronization/finish-scene-message.model';
import { Scene } from '../scene/scene.object';
import { MessageType } from '@aspnet/signalr';
import { SynchronizationMessageType } from 'src/app/shared/models/enum/finish-scene-message-type.enum';
import { NativesCollection } from 'src/app/content/natives-collection';
import { SCENE_FRAME_TIME } from '../engine.helper';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';

@Injectable()
export class SceneService {

  desyncSub = new BehaviorSubject<boolean>(false);
  endGameSub = new Subject<FullSynchronizationInfo>();

  updateSub = new Subject<number>();
  updater;

  private sceneInternal: Scene;

  get scene() {
    return this.sceneInternal;
  }

  // TODO Inject token for natives collection
  constructor(
    private nativesCollection: NativesCollection
  ) { }

  setupGame(fullSynchronizer: FullSynchronizationInfo) {
    this.sceneInternal = new Scene(
      this.desyncSub,
      this.nativesCollection,
      this.endGameSub,
      fullSynchronizer
    );
  }

  startUpdates() {
    clearInterval(this.updater);
    this.scene.lastTime = performance.now();
    this.updater = setInterval(() => {
      const shift = this.scene.update();
      this.updateSub.next(shift);
    }, SCENE_FRAME_TIME * 1000);
  }

  clearScene() {
    clearInterval(this.updater);
    this.sceneInternal = undefined;
  }

  processMessage(message: FinishSceneMessage) {
    if (this.scene && this.scene.id === message.sceneId) {
      this.scene.processMessage(message);
    }
  }
}
