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
export class BattleResolverService implements Resolve<Synchronizer> {

  constructor(
    private userService: UserService,
    private arenaHubService: ArenaHubService,
    private router: Router,
    private webCommunicationService: WebCommunicationService
    ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (!this.userService.user || this.userService.user.state === UserStateEnum.Lobby) {
      this.router.navigate(['lobby']);
      return EMPTY;
    }
    if (this.arenaHubService.prepareForBattleNotifier.value) {
      return undefined;
    }
    return this.webCommunicationService.get<Synchronizer>('api/battle')
      .pipe(map(result => {
        if (!result.result) {
          // TODO Add loading screen with one end
          console.log('Battle resolver error');
        }
        return result.result;
      }));
  }
}
