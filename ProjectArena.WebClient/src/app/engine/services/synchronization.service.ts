import { Injectable } from '@angular/core';
import { SceneService } from './scene.service';
import { SynchronizationMessageDto } from 'src/app/shared/models/synchronization/synchronization-message-dto.model';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';

@Injectable()
export class SynchronizationService {

  constructor(
    private sceneService: SceneService
  ) { }

  issueAction(action: ActionInfo) {

  }

  sendActionSynchronizationInfo(synchronizer: Synchronizer) {

  }

  processMessage(message: SynchronizationMessageDto) {

  }
}
