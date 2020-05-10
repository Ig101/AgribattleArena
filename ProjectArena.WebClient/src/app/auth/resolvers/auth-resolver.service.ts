import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { UserService } from 'src/app/shared/services/user.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';
import { EMPTY } from 'rxjs';

@Injectable()
export class AuthResolverService implements Resolve<boolean> {

  constructor(
    private userService: UserService,
    private router: Router
    ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.userService.user) {
      if (this.userService.user.state === UserStateEnum.Battle) {
        this.router.navigate(['battle']);
      }
      if (this.userService.user.state === UserStateEnum.Lobby) {
        this.router.navigate(['lobby']);
      }
      return EMPTY;
    }
    return true;
  }
}
