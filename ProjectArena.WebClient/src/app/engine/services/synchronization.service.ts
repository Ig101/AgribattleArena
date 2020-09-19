import { Injectable } from '@angular/core';
import { SceneService } from './scene.service';
import { SynchronizationMessageDto } from 'src/app/shared/models/synchronization/synchronization-message-dto.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
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
    sceneService.actionsSub.subscribe(x => this.issueAction(x));
    sceneService.synchronizersSub.subscribe(x => this.sendActionSynchronizationInfo(x));
  }

  private generateNewCode() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      // tslint:disable-next-line: no-bitwise
      const r = Math.random() * 16 | 0;
      // tslint:disable-next-line: no-bitwise
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  issueAction(action: ActionInfo) {
    console.log('act');
    console.log(action);
    // TODO SendAction
    const newCode = this.generateNewCode();
  }

  sendActionSynchronizationInfo(synchronizer: Synchronizer) {
    console.log('sync');
    console.log(synchronizer);
    // TODO SendAction
  }

  processMessage(message: SynchronizationMessageDto) {
    this.sceneService.processMessage(message);
  }
}
