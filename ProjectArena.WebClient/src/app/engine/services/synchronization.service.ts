import { Injectable } from '@angular/core';
import { SceneService } from './scene.service';
import { FinishSceneMessage } from 'src/app/shared/models/synchronization/finish-scene-message.model';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';

@Injectable()
export class SynchronizationService {

  code: string;

  get transformedCode() {
    return this.code;
  }

  constructor(
    private sceneService: SceneService,
    private arenaHubService: ArenaHubService
  ) {
    sceneService.endGameSub.subscribe(message => {
      this.sendFinishSynchronization(message);
    });
  }

  sendFinishSynchronization(message: FullSynchronizationInfo) {

  }

  processMessage(message: FinishSceneMessage) {
    this.sceneService.processMessage(message);
  }
}
