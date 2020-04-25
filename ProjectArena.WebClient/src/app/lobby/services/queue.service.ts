import { Injectable } from '@angular/core';
import { UserManagementService } from './user-management.service';
import { UserService } from 'src/app/shared/services/user.service';

@Injectable()
export class QueueService {

  inQueue = false;
  processingQueueRequest = false;
  timePassed = 0;

  tickingTimer;

  constructor(
    private userManagementService: UserManagementService,
    private userService: UserService
  ) { }

  private timerTick() {
    this.timePassed += 1000;
    console.log('1');
    if (this.timePassed > 1000 * 60 * 59) {
      this.dequeue();
    }
  }

  private setQueue(inQueue: boolean) {
      this.inQueue = inQueue;
      clearInterval(this.tickingTimer);
      if (this.inQueue) {
          this.timePassed = 0;
          this.tickingTimer = setInterval(() => this.timerTick(), 1000);
      }
  }

  enqueue() {
      this.processingQueueRequest = true;
      this.setQueue(true);
      // TODO Connect on server side
  }

  dequeue(): any {
      this.setQueue(false);
      // TODO Dequeue on client side
  }
}
