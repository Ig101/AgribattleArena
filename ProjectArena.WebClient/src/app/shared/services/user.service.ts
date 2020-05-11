import { Injectable } from '@angular/core';
import { ActiveUser } from '../models/active-user.model';
import { Observable, of, Subject } from 'rxjs';
import { WebCommunicationService } from './web-communication.service';
import { map, tap, switchMap } from 'rxjs/operators';
import { ExternalResponse } from '../models/external-response.model';
import { ArenaHubService } from './arena-hub.service';

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
      this.user.tavern = info.tavern;
      console.log('send');
      this.userChanged.next();
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
        this.email = this.user.email;
        return this.arenaHub.connect(this.user.id).pipe(map(connect => result));
      } else {
        this.unauthorized = true;
        return of(result);
      }
    }));
  }
}
