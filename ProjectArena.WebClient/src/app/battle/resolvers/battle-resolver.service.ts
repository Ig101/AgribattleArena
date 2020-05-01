import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserService } from 'src/app/shared/services/user.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';
import { EMPTY } from 'rxjs';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { map } from 'rxjs/operators';

@Injectable()
export class BattleResolverService implements Resolve<boolean> {

  private battleSnapshot: Synchronizer;

  constructor(
    private userService: UserService,
    private arenaHubService: ArenaHubService,
    private router: Router,
    private webCommunicationService: WebCommunicationService
    ) { }

  popBattleSnapshot() {
    const snapshot = this.battleSnapshot;
    this.battleSnapshot = undefined;
    return snapshot;
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (!this.userService.user || this.userService.user.state === UserStateEnum.Lobby) {
      this.router.navigate(['lobby']);
      return EMPTY;
    }
    if (this.arenaHubService.prepareForBattleNotifier.value) {
      this.battleSnapshot = undefined;
      return false;
    }
    return this.webCommunicationService.get<Synchronizer>('api/battle')
      .pipe(map(result => {
        if (!result.result) {
          // TODO Add loading screen with one end
          console.log('Battle resolver error');
        }
        this.battleSnapshot = result.result;
        return true;
      }));
  }
}
