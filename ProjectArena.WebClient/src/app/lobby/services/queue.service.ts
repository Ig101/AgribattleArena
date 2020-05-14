import { Injectable } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { Subscription, Subject } from 'rxjs';

@Injectable()
export class QueueService {

  inQueue = false;
  exiting = false;
  processingQueueRequest = false;
  timePassed = 0;

  tickingTimer;

  queueSeed: number;
  ticksTillNewSeed = 1;
  ticks: number;

  queueUpdate = new Subject<any>();

  private queueSubscription: Subscription;
  private waitingForDequeue = false;
  private waitingForEnqueue = false;

  constructor(
    private loadingService: LoadingService,
    private userService: UserService,
    private webCommunicationService: WebCommunicationService
  ) { }

  private timerTick() {
    this.timePassed += 1000;
    this.ticks -= 1;
    if (this.ticks <= 0) {
      this.setRandomQueueSeed();
    }
    if (this.timePassed > 1000 * 60 * 59) {
      this.dequeue();
    }
  }

  private setRandomQueueSeed() {
    this.ticks = this.ticksTillNewSeed;
    this.queueSeed = Math.floor(Math.random() * 1000) + 1;
    this.queueUpdate.next();
  }

  private setQueue(inQueue: boolean) {
    this.inQueue = inQueue;
    clearInterval(this.tickingTimer);
    if (this.inQueue) {
      this.exiting = false;
      this.timePassed = 0;
      this.setRandomQueueSeed();
      this.tickingTimer = setInterval(() => this.timerTick(), 1000);
    }
  }

  enqueue() {
      this.processingQueueRequest = true;
      this.setQueue(true);
      if (this.queueSubscription) {
        this.waitingForEnqueue = true;
      } else {
      this.queueSubscription = this.webCommunicationService.post('api/queue', null)
          .subscribe(result => {
              if (!result.success) {
                this.loadingService.startLoading({
                  title: result.errors[0]
                }, undefined, true);
                setTimeout(() => {
                  location.reload();
                }, 2000);
              }
              this.queueSubscription.unsubscribe();
              this.queueSubscription = undefined;
              if (this.waitingForDequeue) {
                this.waitingForDequeue = false;
                this.dequeue();
              }
              if (this.waitingForEnqueue) {
                this.waitingForEnqueue = false;
              }
            }
          );
      }
  }

  dequeue(silent = false): any {
    if (!this.inQueue) {
      this.setQueue(false);
      return;
    }
    this.exiting = true;
    if (this.queueSubscription) {
      this.waitingForDequeue = true;
    } else {
      this.queueSubscription = this.webCommunicationService.delete('api/queue')
        .subscribe(result => {
            if (!result.success && !silent) {
              this.loadingService.startLoading({
                title: result.errors[0]
              }, undefined, true);
              setTimeout(() => {
                location.reload();
              }, 2000);
            }
            this.setQueue(false);
            this.queueSubscription.unsubscribe();
            this.queueSubscription = undefined;
            this.queueUpdate.next();
            if (this.waitingForEnqueue) {
              this.waitingForEnqueue = false;
              this.enqueue();
            }
            if (this.waitingForDequeue) {
              this.waitingForDequeue = false;
            }
          }
        );
    }
  }
}
