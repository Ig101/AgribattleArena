import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TalentNode } from '../ascii/model/talent-node.model';
import { UserService } from 'src/app/shared/services/user.service';

@Injectable()
export class TalentsService {

  constructor(
    private userService: UserService
  ) { }

  getTalentsMap(): TalentNode[][] {
    const talents = this.userService.user.talentsMap;
    const map = new Array<TalentNode[]>(49);
    for (let i = 0; i < 49; i++) {
      map[i] = new Array<TalentNode>(25);
    }

    for (const talent of talents) {
      map[talent.x][talent.y] = talent;
    }

    return map;
  }
}
