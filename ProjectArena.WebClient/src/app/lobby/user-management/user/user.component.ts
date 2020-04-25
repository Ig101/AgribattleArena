import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserManagementService } from '../../services/user-management.service';
import { UserService } from 'src/app/shared/services/user.service';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { Router } from '@angular/router';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';
import { QueueService } from '../../services/queue.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit, OnDestroy {

  get userName() {
    return this.userService.user.name;
  }

  get userUniqueId() {
    return this.userService.user.uniqueId;
  }

  get inQueue() {
    return this.queueService.inQueue;
  }

  get queueTimePassed() {
    return this.queueService.timePassed;
  }

  get exitingFromQueue() {
    return this.queueService.exiting;
  }

  componentSizeEnum = ComponentSizeEnum;
  userStateEnum = UserStateEnum;

  constructor(
    private userManagementService: UserManagementService,
    private queueService: QueueService,
    private userService: UserService,
    private webCommunicationService: WebCommunicationService,
    private router: Router
  ) { }

  ngOnDestroy(): void {
    this.queueService.dequeue();
  }

  ngOnInit(): void {
    setTimeout(() => this.userManagementService.loadingEnd());
  }

  searchMatch() {
    this.queueService.enqueue();
  }

  cancelSearch() {
    this.queueService.dequeue();
  }

  toUserSettings() {
    this.router.navigate(['lobby/settings']);
  }

  logOff() {
    this.userManagementService.loadingStart();
    this.webCommunicationService.delete('api/auth/signout')
    .subscribe((result) => {
      if (result.success) {
        this.userService.unauthorized = true;
        this.userService.user = undefined;
        this.router.navigate(['lobby/signin']);
      } else {
        this.userManagementService.loadingError(result.errors);
      }
    });
  }
}
