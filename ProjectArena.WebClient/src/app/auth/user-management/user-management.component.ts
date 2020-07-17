import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { UserManagementWindowEnum } from '../models/enum/user-management-window.enum';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { FormGroup } from '@angular/forms';
import { UserManagementService } from '../services/user-management.service';
import { Subscription } from 'rxjs';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { Router } from '@angular/router';
import { UserService } from 'src/app/shared/services/user.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';
import { LoadingService } from 'src/app/shared/services/loading.service';

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
    private router: Router,
    private loadingService: LoadingService
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
      this.userManagementService.loadingError(['Server connection is lost. Refresh the page or try again later...'], false);
      console.error('Hub connection is lost');
      this.userService.unauthorized = true;
      this.userService.user = undefined;
      this.router.navigate(['auth/signin']);
    });
    this.hubBattleSubscription = this.arenaHubService.prepareForBattleNotifier.subscribe((value) => {
      if (value) {
        this.userService.user.state = UserStateEnum.Battle;
        this.loadingService.startLoading({
          title: 'Encountered enemy. Prepare for battle!',
          loadingScene: value
        }, 300000).subscribe(() => {
          this.router.navigate(['battle']);
        });
      }
    });
    this.hubErrorSubscription = this.arenaHubService.synchronizationErrorState.subscribe((value) => {
      if (value) {
        this.loadingService.startLoading({
          title: 'Desynchronization. Page will be refreshed in 2 seconds.'
        }, 0, true);
        console.error('Unexpected synchronization error');
        setTimeout(() => {
          location.reload();
        }, 2000);
      }
    });
  }
}
