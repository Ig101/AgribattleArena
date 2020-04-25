import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserManagementWindowEnum } from '../models/enum/user-management-window.enum';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { FormGroup } from '@angular/forms';
import { UserManagementService } from '../services/user-management.service';
import { Subscription } from 'rxjs';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { Router } from '@angular/router';
import { UserService } from 'src/app/shared/services/user.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit, OnDestroy {

  userManagementWindowEnum = UserManagementWindowEnum;
  userState: UserManagementWindowEnum = this.userManagementWindowEnum.SignIn;

  hubSubscription: Subscription;

  get loading() {
    return this.userManagementService.loading;
  }

  constructor(
    private userManagementService: UserManagementService,
    private userService: UserService,
    private arenaHubService: ArenaHubService,
    private router: Router
    ) { }

  ngOnDestroy(): void {
    if (this.hubSubscription) {
      this.hubSubscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.hubSubscription = this.arenaHubService.onClose.subscribe(() => {
      this.userManagementService.loadingError(['Disconnected from server. Try to refresh the page.'], false);
      this.userService.unauthorized = true;
      this.userService.user = undefined;
      this.router.navigate(['lobby/signin']);
    });
  }
}
