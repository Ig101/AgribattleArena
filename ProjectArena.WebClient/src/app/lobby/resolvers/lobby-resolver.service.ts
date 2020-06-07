import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { UserService } from 'src/app/shared/services/user.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';
import { EMPTY } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class LobbyResolverService implements Resolve<boolean> {

  private battleSnapshot: Synchronizer;

  constructor(
    private userService: UserService,
    private arenaHubService: ArenaHubService,
    private router: Router,
    private webCommunicationService: WebCommunicationService,
    ) { }

  popBattleSnapshot() {
    const snapshot = this.battleSnapshot;
    this.battleSnapshot = undefined;
    return snapshot;
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (!this.userService.user) {
      this.router.navigate(['auth']);
      return EMPTY;
    }
    if (this.userService.user.state === UserStateEnum.Battle) {
      this.router.navigate(['battle']);
      return EMPTY;
    }
    return true;
  }
}
