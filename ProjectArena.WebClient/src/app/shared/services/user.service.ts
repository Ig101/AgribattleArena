import { Injectable } from '@angular/core';
import { ActiveUser } from '../models/active-user.model';
import { Observable, of, Subject } from 'rxjs';
import { WebCommunicationService } from './web-communication.service';
import { map, tap, switchMap } from 'rxjs/operators';
import { ExternalResponse } from '../models/external-response.model';
import { ArenaHubService } from './arena-hub.service';
import { CharacterClassEnum } from 'src/app/lobby/ascii/model/enums/character-class.enum';
import { getNativeIdByTalents } from 'src/app/helpers/talents.helper';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  unauthorized = false;
  user: ActiveUser;
  userChanged = new Subject<any>();

  email: string;

  passwordWasChanged = false;
  emailWasConfirmed = false;

  constructor(
    private webCommunicationService: WebCommunicationService,
    private arenaHub: ArenaHubService
  ) {
    arenaHub.dailyUpdateNotifier.subscribe(info => {
      let userChanged = false;
      if (info.tavern) {
        this.user.tavern = info.tavern;
        userChanged = true;
      }
      if (userChanged) {
        this.userChanged.next();
      }
    });
  }

  getActiveUser() {
    if (this.user) {
      return of({
        success: true,
        statusCode: 200,
        result: this.user
      } as ExternalResponse<ActiveUser>);
    }
    return this.webCommunicationService.get<ActiveUser>('api/user')
    .pipe(switchMap(result => {
      this.user = result.result;
      if (this.user) {
        for (const character of this.user.roster) {
          const characterTalents = this.user.talentsMap.filter(x => character.talents.some(k => k.x === x.x && k.y === x.y));
          character.nativeId = getNativeIdByTalents(characterTalents);
        }
        this.email = this.user.email;
        return this.arenaHub.connect(this.user.id).pipe(map(connect => result));
      } else {
        this.unauthorized = true;
        return of(result);
      }
    }));
  }
}
