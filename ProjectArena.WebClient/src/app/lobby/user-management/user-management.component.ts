import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserManagementWindowEnum } from '../models/enum/user-management-window.enum';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { FormGroup } from '@angular/forms';
import { UserManagementService } from '../services/user-management.service';
import { Subscription } from 'rxjs';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { Router } from '@angular/router';
import { UserService } from 'src/app/shared/services/user.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit, OnDestroy {

  userManagementWindowEnum = UserManagementWindowEnum;
  userState: UserManagementWindowEnum = this.userManagementWindowEnum.SignIn;

  hubCloseSubscription: Subscription;
  hubBattleSubscription: Subscription;
  hubErrorSubscription: Subscription;

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
    if (this.hubCloseSubscription) {
      this.hubCloseSubscription.unsubscribe();
    }
    if (this.hubBattleSubscription) {
      this.hubBattleSubscription.unsubscribe();
    }
    if (this.hubErrorSubscription) {
      this.hubErrorSubscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.hubCloseSubscription = this.arenaHubService.onClose.subscribe(() => {
      this.userManagementService.loadingError(['Disconnected from server. Try to refresh the page.'], false);
      this.userService.unauthorized = true;
      this.userService.user = undefined;
      this.router.navigate(['lobby/signin']);
    });
    this.hubBattleSubscription = this.arenaHubService.prepareForBattleNotifier.subscribe((value) => {
      if (value) {
        this.userService.user.state = UserStateEnum.Battle;
        this.router.navigate(['battle']);
      }
    });
    this.hubErrorSubscription = this.arenaHubService.synchronizationErrorState.subscribe((value) => {
      if (value) {
        // TODO Error death screen
        console.log('Synchronization error.');
      }
    });
  }
}
