import { Injectable } from '@angular/core';
import { UserManagementService } from './user-management.service';
import { UserService } from 'src/app/shared/services/user.service';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';

@Injectable()
export class QueueService {

  inQueue = false;
  exiting = false;
  processingQueueRequest = false;
  timePassed = 0;

  tickingTimer;

  constructor(
    private userManagementService: UserManagementService,
    private userService: UserService,
    private webCommunicationService: WebCommunicationService
  ) { }

  private timerTick() {
    this.timePassed += 1000;
    if (this.timePassed > 1000 * 60 * 59) {
      this.dequeue();
    }
  }

  private setQueue(inQueue: boolean) {
    this.inQueue = inQueue;
    clearInterval(this.tickingTimer);
    if (this.inQueue) {
      this.exiting = false;
      this.timePassed = 0;
      this.tickingTimer = setInterval(() => this.timerTick(), 1000);
    }
  }

  enqueue() {
      this.processingQueueRequest = true;
      this.setQueue(true);
      this.webCommunicationService.post('api/queue', null)
        .subscribe(result => {
            if (!result.success) {
              this.userManagementService.loadingError(result.errors);
            }
          }
        );
  }

  dequeue(silent = false): any {
    this.exiting = true;
    this.webCommunicationService.delete('api/queue')
      .subscribe(result => {
          if (!result.success && !silent) {
            this.userManagementService.loadingError(result.errors);
          }
          this.setQueue(false);
        }
      );
  }
}
